using Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratorsTest
{
    [TestClass]
    public class GenericGeneratorTests
    {
        [TestMethod]
        public void TestGeneration()
        {
            string input = @"
func: 'a1: ' + {1}; + '--
    a2: ' + {2}

baseheight: 40

type: td
    height: {baseheight}px
    width: 60px

type: tr
    color: white

class: container
    background: yellow
    height: ({baseheight} + 20)px

style: funcStyle
    2: func:(a3|a4)

style: simple
    color: pink
";

            string template = @"
dummy: 1

OutputItems: COMP:(COUNT:({{1}})|0|''|SUM:OVER:({{1}}|x|{2}{x} + ' {' + SUM:OVER:({{1}={x}.ALL.NAME}|y|' --
    ' + COMP:({y}|INT:({y})|''|{y}  + ': ') + {{1}={x}.{y}} + ';') + '--
} --
' + '--
'))

Output: OutputItems:(type|'') + OutputItems:(class|'.') + OutputItems:(style|'#')";

            string expected = @"td {
    height: 40px;
    width: 60px;
}

tr {
    color: white;
}

.container {
    background: yellow;
    height: 60px;
}

#funcStyle {
    a1: a3;
    a2: a4;
}

#simple {    color: pink;}".Replace("\n", "\r").Replace("\r\r", "\n").Replace("\r", "\n"); // Something odd when creating expected so using this to regularise.

            GenericGenerator generator = new GenericGenerator();
            string generated = generator.Generate(input, template).Replace("\r", "");
            string soFar = "";
            //Assert.AreEqual(expected, generated);
            for (int i = 0; i < expected.Length; ++i)
            {
                char e = expected[i];
                char g = generated[i];
                soFar += e;
                Assert.AreEqual(e, g, $"error at {i}, '{e}'[{(int)e}] != '{g}'[{(int)g}]\n'{soFar}'");
            }
        }
    }
}
