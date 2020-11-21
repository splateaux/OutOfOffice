using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SoftProGameWindows
{
    /// <summary>
    /// Creates a sprite given the specific parameters.
    /// </summary>
    public static class SpriteFactory
    {
        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="type">The type of the sprite to create.</param>
        /// <param name="info">The sprite information used to initialize the sprite.</param>
        /// <param name="useCache">if set to <c>true</c> cache the results.</param>
        /// <returns>An instantiated <see cref="Sprite"/>.</returns>
        public static Sprite Create(Type type, SpriteInfo info)
        {
            MethodInfo method = typeof(SpriteFactory).GetMethod("Create", new Type[0]);
            MethodInfo generic = method.MakeGenericMethod(type);
            var func = (Func<SpriteInfo, Sprite>)generic.Invoke(null, null);
            return func(info);
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="useCache">if set to <c>true</c> cache the results.</param>
        /// <returns></returns>
        public static Func<SpriteInfo, T> Create<T>()
        {
            Type objType = typeof(T);
            Type[] types = new[] { typeof(SpriteInfo) };
            var dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + objType.Name, objType, types, objType);

            // if need more than 1 arg add another Ldarg_x
            // you'll also need to add proper generics and 
            // CreateDelegate signatures
            ILGenerator ilGen = dynMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Newobj, objType.GetConstructor(types));
            ilGen.Emit(OpCodes.Ret);
            return (Func<SpriteInfo, T>)dynMethod.CreateDelegate(typeof(Func<SpriteInfo, T>));
        }
    }
}
