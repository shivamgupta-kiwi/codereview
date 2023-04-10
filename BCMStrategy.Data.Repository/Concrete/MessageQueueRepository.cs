using Amazon.SQS;
using Amazon.SQS.Model;
using BCMStrategy.Data.Abstract.Abstract;
using BCMStrategy.Data.Abstract.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using BCMStrategy.Data.Abstract;

namespace BCMStrategy.Data.Repository.Concrete
{
  public class MessageQueueRepository : IMessageQueue
  {
    /// <summary>
    /// Send Message to the Queue
    /// </summary>
    /// <param name="messageRequest">Send respective message to the Queue</param>
    /// <returns>Returns status code</returns>
    public bool SendMessage(MessageQueue message)
    {
      SendMessageRequest messageRequest = new SendMessageRequest();
      bool result = false;

      string queueURL = GetQueueURL(message);
      
      string accessKey = Helper.GetSQSAWSAccessKeyId();
      string accessValue = Helper.GetSQSAWSSecretAccessKey();

      AmazonSQSClient amazonSQSClient = new AmazonSQSClient(accessKey, accessValue);

      messageRequest.QueueUrl = queueURL;
      messageRequest.MessageBody = message.MessageBody;

      SendMessageResponse response = amazonSQSClient.SendMessage(messageRequest);

      if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
      {
        result = true;
      }

      return result;
    }

    /// <summary>
    /// Delete the message from the Queue
    /// </summary>
    /// <param name="message">Passes message information</param>
    /// <returns>Status Code</returns>
    public bool DeleteMessage(MessageQueue message)
    {

      bool result = false;

      string queueURL = GetQueueURL(message);
			string accessKey = Helper.GetSQSAWSAccessKeyId();
			string accessValue = Helper.GetSQSAWSSecretAccessKey();

			var request = new ReceiveMessageRequest
      {
        AttributeNames = new List<string>() { "All" },
        MaxNumberOfMessages = 5,
        QueueUrl = queueURL
      };

      AmazonSQSClient amazonSQSClient = new AmazonSQSClient(accessKey, accessValue);

      Message messageList = amazonSQSClient.ReceiveMessage(request).Messages.Find(x => x.Body.Equals(message.MessageBody));

      if (messageList != null)
      {
        var delRequest = new DeleteMessageRequest
        {
          QueueUrl = queueURL,
          ReceiptHandle = messageList.ReceiptHandle
        };

        var delResponse = amazonSQSClient.DeleteMessage(delRequest);

        if (delResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
          result = true;
        }
      }

      return result;
    }

    /// <summary>
    /// Read the Message and then Delete it and return the message to the application for processing further
    /// </summary>
    /// <param name="message">Message Queue</param>
    /// <returns>Returns the Message</returns>
    public string ReadAndDeleteMessage(MessageQueue message)
    {
      string messageBody = string.Empty;

      string queueURL = GetQueueURL(message);

			string accessKey = Helper.GetSQSAWSAccessKeyId();
			string accessValue = Helper.GetSQSAWSSecretAccessKey();

			var request = new ReceiveMessageRequest
      {
        AttributeNames = new List<string>() { "All" },
        MaxNumberOfMessages = 1,
        QueueUrl = queueURL,
        VisibilityTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds,
        WaitTimeSeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds
      };
      
      AmazonSQSClient amazonSQSClient = new AmazonSQSClient(accessKey, accessValue);

      var response = amazonSQSClient.ReceiveMessage(request);

      if (response.Messages.Count > 0)
      {
        foreach (var messageQueue in response.Messages)
        {
          var delRequest = new DeleteMessageRequest
          {
            QueueUrl = queueURL,
            ReceiptHandle = messageQueue.ReceiptHandle
          };

          var delResponse = amazonSQSClient.DeleteMessage(delRequest);

          if (delResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
          {
            messageBody = messageQueue.Body;
          }
        }
      }

      return messageBody;
    }

    public bool ReadQueueMessage(MessageQueue messageQueue)
    {
      bool result = false;

      ////string messageBody = string.Empty;

      string queueURL = GetQueueURL(messageQueue);

			string accessKey = Helper.GetSQSAWSAccessKeyId();
			string accessValue = Helper.GetSQSAWSSecretAccessKey();

			var request = new ReceiveMessageRequest
      {
        AttributeNames = new List<string>() { "All" },
        MaxNumberOfMessages = 1,
        QueueUrl = queueURL
      };

      AmazonSQSClient amazonSQSClient = new AmazonSQSClient(accessKey, accessValue);

      var response = amazonSQSClient.ReceiveMessage(request);

      if (response.Messages.Count > 0)
      {
        result = true;
      }

      return result;
    }

		public int ReadQueueMessageCount(MessageQueue messageQueue)
		{
      ////string messageBody = string.Empty;

			string queueURL = GetQueueURL(messageQueue);

			string accessKey = Helper.GetSQSAWSAccessKeyId();
			string accessValue = Helper.GetSQSAWSSecretAccessKey();

			var request = new ReceiveMessageRequest
			{
				AttributeNames = new List<string>() { "All" },
				QueueUrl = queueURL,
				MaxNumberOfMessages=10,
				VisibilityTimeout=0
			};
			
			AmazonSQSClient amazonSQSClient = new AmazonSQSClient(accessKey, accessValue);

			var response = amazonSQSClient.ReceiveMessage(request);


			return response.Messages.Count;
		}

		/// <summary>
		/// Get the Queue URL
		/// </summary>
		/// <param name="messageQueue">Pass the Queue Type</param>
		/// <returns>Returns the URL</returns>
		public string GetQueueURL(MessageQueue messageQueue)
    {
      string queueURL = string.Empty;

      switch (messageQueue.QueueType)
      {
        case 1:
          queueURL = Helper.GetSQSContentLoader();
          break;
        case 2:
          queueURL = Helper.GetSQSEmailGeneration();
          break;
        default:
          queueURL = Helper.GetSQSContentLoader();
					break;
      }

      return queueURL;
    }
  }
}