using CollabApp.mvc.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabApp.UnitTests.Utilities
{
    public class IdGeneratorTests
    {
        [Fact]
        public void GenerateUniqueId_ShouldReturnUniqueId()
        {
            // Arrange
            HashSet<int> usedIds = new HashSet<int>();

            // Act
            int id = IdGenerator.GenerateUniqueId(usedIds);
            int id2 = IdGenerator.GenerateUniqueId(usedIds);

            // Assert
            Assert.NotEqual(id, id2);
        }
    }
}
