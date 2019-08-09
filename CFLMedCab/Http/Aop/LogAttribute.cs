using CFLMedCab.Infrastructure.ToolHelper;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using System;
using System.Reflection;
using System.Text;


// This file contains registration of aspects that are applied to several classes of this project.
/*AttributeTargetTypes 要拦截过滤的类
 * AttributeTargetMembers = “Dispose”，AttributeExclude = true 排除掉任何名为Dispose的方法
 * 
*AttributeTargetTypeAttributes 拦截的类属性
*AttributeTargetMemberAttributes 拦截的方法属性
*/
[assembly: CFLMedCab.Http.Aop.Log(
	AttributeTargetTypes = "null",
	AttributeTargetTypeAttributes = MulticastAttributes.Public,
	AttributeTargetMemberAttributes = MulticastAttributes.Public
)]
namespace CFLMedCab.Http.Aop
{
	/*针对整个方法体进行包围调用添加日志和截取异常(类似于spring中的环绕通知)
        * 
        * PostSharp中的MethodInterceptionAspect类是针对整个方法体的截取，
        * 继承于它的特性可以对整个方法体进行控制和日志截取、异步操作等!
        * 这个类里面有一个主要的函数可以重载以实现包围整个方法体截取的作用，
        * 它是OnInvoke(MethodInterceptionArgs args)。意义如下：
        * 在它的内部可以通过base.OnInvoke(args)来调用我们加特性声明的方法执行流程,
        * 通过这个方法我们可以在方法开始调用前做操作，调用之后做操作。
        */
	[Serializable]
	public class LogAttribute : MethodInterceptionAspect
	{
		public override void OnInvoke(MethodInterceptionArgs args)
		{
			Arguments arguments = args.Arguments;
			StringBuilder sb = new StringBuilder();
			ParameterInfo[] parameters = args.Method.GetParameters();
			for (int i = 0; arguments != null && i < arguments.Count; i++)
			{
				//进入的参数的值        
				sb.Append(parameters[i].Name + "=" + arguments[i] + "");
			}
			try
			{
				LogUtils.Debug($"进入{args.Method.DeclaringType}.{args.Method.Name}方法，参数是:{sb.ToString()}");
				DateTime methodStartTime = DateTime.Now;
				base.OnInvoke(args);
				DateTime methodEndTime = DateTime.Now;
				LogUtils.Debug($"{args.Method.DeclaringType}.{args.Method.Name}方法耗时{methodEndTime.Subtract(methodStartTime).TotalMilliseconds}毫秒");
				LogUtils.Debug($"退出{args.Method.DeclaringType }.{ args.Method.Name}方法，返回结果是:{args.ReturnValue}");
			}
			catch (Exception ex)
			{
				LogUtils.Debug(string.Format($"出现异常，此方法异常信息是：{ex.ToString()}"));
			}
		}
	}

}
