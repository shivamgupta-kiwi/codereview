using BCMStrategy.Data.Abstract.ViewModels;

namespace BCMStrategy.Data.Abstract.Abstract
{
  public interface IMessageQueue
  {
    /// <summary>
    /// Send Message to the Queue
    /// </summary>
    /// <param name="message">Message to be pass</param>
    /// <returns>Boolean value whether message has been send successfully or not</returns>
    bool SendMessage(MessageQueue message);

    /// <summary>
    /// Delete Message from the Queue
    /// </summary>
    /// <param name="message">Message to be pass</param>
    /// <returns>Boolean value whether message has been deleted successfully or not</returns>
    bool DeleteMessage(MessageQueue message);

    /// <summary>
    /// Read Specific Message from the Queue
    /// </summary>
    /// <param name="message">Message to read</param>
    /// <returns>Boolean value whether message has been read successfully or not</returns>
    string ReadAndDeleteMessage(MessageQueue message);

    /// <summary>
    /// Read Specific Message from the Queue
    /// </summary>
    /// <param name="message">Message to read</param>
    /// <returns>Boolean value whether message has been read successfully or not</returns>
    bool ReadQueueMessage(MessageQueue messageQueue);

		int ReadQueueMessageCount(MessageQueue messageQueue);

	}
}