using System;
using System.Diagnostics;
using BlazorApp.Client.Shared;

namespace RudeTests;

[TestClass]
public class ChatBoxTests : BunitTestContext
{
    [TestMethod]
	public void ModelIsEmptyAtInitialization()
	{
        var cut = RenderComponent<ChatBox>();
        
		// Arrange
		Assert.AreEqual("", cut.Instance.Model);
	}

    [TestMethod]
	public void HandleSendButtonClickSuccess()
	{
        var cut = RenderComponent<ChatBox>();
        cut.Find("textarea").Input("Hello World");

        Assert.AreEqual("Hello World", cut.Instance.Model);       
		cut.Find("button").Click();
        Assert.AreEqual("", cut.Instance.Model); 

        //string textarea = "<textarea class=\"textarea\" @bind=\"\" \u0040onkeydown:preventDefault=\"true\" \u0040bind:event=\"oninput\" \u0040onkeydown=\"HandleKeyDown\"></textarea>";
        //Debug.WriteLine(textarea);
        //cut.Find("textarea").MarkupMatches("<textarea class=\"textarea\" @bind=\"\" \u0040onkeydown:preventDefault=\"true\" \u0040bind:event=\"oninput\" \u0040onkeydown=\"HandleKeyDown\"></textarea>");     
	}
}
