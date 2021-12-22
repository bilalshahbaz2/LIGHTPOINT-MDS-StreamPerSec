using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Marker
{
    public interface IMarker {

        void init(IServiceCollection services);

        void beforeInit();

        void afterInit();

    }

    public abstract class BaseMarker : IMarker
    {
        public BaseMarker()
        {

        }

        public virtual void afterInit()
        {
         
        }

        public virtual void beforeInit()
        {
        }

        public virtual void init(IServiceCollection services)
        {

        }

        public static void Init<M>(IServiceCollection services)
            where M : IMarker
        {
            var m = (M)Activator.CreateInstance(typeof(M));
           
            m.init(services);
        }

       
    }



}
