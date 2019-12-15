using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAssembly;
using WebAssembly.Instructions;
using WebAssembly.Runtime;

namespace WebAssembly4b.Tests {


  /// <summary>
  /// A simple test class.
  /// </summary>
  public abstract class HelloWorldExports {
    /// <summary>
    /// A simple test method.
    /// </summary>
    /// <returns>Should always return "3".</returns>
    public abstract int Start();
  }

  class Program {
    static void Main(string[] args) {

      var module = new Module();
      module.Types.Add(new WebAssemblyType {
        Returns = new[]
          {
                    WebAssemblyValueType.Int32,
                }
      });
      module.Functions.Add(new Function {
      });
      module.Exports.Add(new Export {
        Name = nameof(HelloWorldExports.Start)
      });
      module.Codes.Add(new FunctionBody {
        Code = new Instruction[]
          {
                new Int32Constant { Value = 3 },
                new End(),
          },
      });

      var compiled = module.Compile<HelloWorldExports>();

      //var exports = compiled.Exports;
      //Assert.AreEqual(3, exports.Start());


    }
  }
}
