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
	}

    [TestMethod]
    public void SendButtonRenderedCorrectly() {
        var cut = RenderComponent<ChatBox>();
        cut.Find("button").MarkupMatches(@"<button class=""button is-primary is-block mr-3 is-rounded""  >Send</button>");
    }
}
