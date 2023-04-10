using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using Unity;

namespace BCMStrategy.Common.Unity
{
  public class UnityHelper
  {
    private static IUnityContainer _container;

    /// <summary>
    /// Gets the container with configured types
    /// </summary>
    public static IUnityContainer Container
    {
      get
      {
        if (_container == null)
        {
          _container = new UnityContainer();

          ConfigureDependency(_container);
        }

        return _container;
      }
    }

    /// <summary>
    /// Configures types from unity.config file
    /// </summary>
    /// <param name="container"></param>
    private static void ConfigureDependency(IUnityContainer container)
    {
      var unitySection = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

      container.LoadConfiguration(unitySection);
    }

    /// <summary>
    /// Resolves specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Resolve<T>()
    {
      return Container.Resolve<T>();
    }

    /// <summary>
    /// Resolves named instance specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Resolve<T>(string name)
    {
      return Container.Resolve<T>(name);
    }

    /// <summary>
    /// Registers specified instance for specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    public static void RegisterInstance<T>(T instance)
    {
      Container.RegisterInstance<T>(instance);
    }
  }
}