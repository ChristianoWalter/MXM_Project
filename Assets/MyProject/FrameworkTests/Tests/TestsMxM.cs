using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestsMxM
{

    [Test]
    public void TestPlayerHealth()
    {
         // Arrange
        float health = 100;

        // Act
        health -= 10;

        // Assert
        Assert.AreEqual(90, health);

    }

}
