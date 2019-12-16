using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAssembly;
using WebAssembly.Runtime;
using WebAssembly.Instructions;
using System.Reflection.Emit;

namespace UseDLL {
  public interface Exports {
    void main();
  }

  /// <summary>
  /// Many compiler tests can use this template to host the test.
  /// </summary>
  public abstract class CompilerTestBase<T>
      where T : struct {
    /// <summary>
    /// Creates a new <see cref="CompilerTestBase{T}"/> instance.
    /// </summary>
    protected CompilerTestBase() {
    }

    /// <summary>
    /// Returns a value.
    /// </summary>
    /// <param name="parameter">Input to the test function.</param>
    /// <returns>A value to ensure proper control flow and execution.</returns>
    public abstract T Test(T parameter);

    /// <summary>
    /// Provides a <see cref="CompilerTestBase{T}"/> for the provided instructions.
    /// </summary>
    /// <param name="instructions">The instructions that form the body of the <see cref="Test(T)"/> function.</param>
    /// <returns>The <see cref="CompilerTestBase{T}"/> instance.</returns>
    public static void CreateInstance(params Instruction[] instructions) {
      var type = WebAssembly.AssemblyBuilder.Map(typeof(T));

      WebAssembly.AssemblyBuilder.CreateInstance<CompilerTestBase<T>>(nameof(CompilerTestBase<T>.Test),
          type,
          new[]
          {
                    type,
          },
          instructions);
    }
  }

  public class Program {

    static void Main(string[] args) {
      using (var tr = new System.IO.FileStream(@"helloworld.wasm", System.IO.FileMode.Open)) {

        var module = new Module();
        module.Types.Add(new WebAssemblyType {
          Returns = new[] { WebAssemblyValueType.Int32 },
          Parameters = new[] { WebAssemblyValueType.Int32 }
        });
        module.Imports.Add(new Import.Table {
          Module = "Test",
          Field = "Test",
          Definition = new Table {
            ElementType = ElementType.FunctionReference,
            ResizableLimits = new ResizableLimits(1)
          }
        });
        module.Functions.Add(new Function {
        });
        module.Exports.Add(new Export {
          Name = "Test",
        });
        module.Elements.Add(new Element {
          Elements = new uint[] { 0 },
          InitializerExpression = new Instruction[]
            {
                    new Int32Constant(0),
                    new End(),
            },
        });
        module.Codes.Add(new FunctionBody {
          Code = new Instruction[]
            {
                    new LocalGet(0),
                    new End()
            },
        });

        var table = new FunctionTable(1);

         module.ToInstance<CompilerTestBase<int>>(
            new ImportDictionary {
                    { "Test", "Test", table },
            });

      }


    }
  }
}
