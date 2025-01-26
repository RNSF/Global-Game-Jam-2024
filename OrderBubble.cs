using Godot;
using System;



	
[Tool]
public partial class OrderBubble : Control
{

	


	Vector2 _padding;
	[Export]
	public Vector2 Padding {
		get => _padding;
		set {
			_padding = value;
			UpdateSize();
		}
	}

	public Control Bubble {
		get => GetNode<Control>("Bubble");
	}

	public RichTextLabel BubbleLabel {
		get => GetNode<RichTextLabel>("Bubble/RichTextLabel");
	}


	[Export(PropertyHint.Range, "0.0, 1.0, 0.01")]
	public float PercentVisible {
		get => BubbleLabel.VisibleRatio;
		set {
			BubbleLabel.VisibleRatio = value;
			UpdateSize();
		}
	}

	[Export(PropertyHint.MultilineText)]
	public String Text {
		get => BubbleLabel.Text;

		set {
			BubbleLabel.Text = value;
			UpdateSize();
		}	
	}


	public void UpdateSize() {
		Font font = BubbleLabel.GetThemeFont("normal_font");
		int fontSize = BubbleLabel.GetThemeFontSize("normal_font_size");
		String rawText = StripBBCode(BubbleLabel.Text);
		rawText = rawText.Substring(0, (int) (BubbleLabel.VisibleRatio * rawText.Length));
		var textSize = new Vector2(50, 0);
		var lines = rawText.Split("\n");
		if (rawText.Length == 0) rawText = "0";
		foreach(var line in lines) {
			var nextTextSize = font.GetStringSize(line, HorizontalAlignment.Center, -1, fontSize);
			textSize.X = Math.Max(textSize.X, nextTextSize.X);
		}

		textSize.Y = font.GetStringSize(rawText, HorizontalAlignment.Center, -1, fontSize).Y * lines.Length;

		Bubble.Size = textSize + Padding;
		Bubble.Position = Bubble.Size * new Vector2(-0.5f, -1f);

		BubbleLabel.Size = textSize;
		BubbleLabel.Position = Padding / 2;
	}

	String StripBBCode(String source) {
		var regex = new RegEx();
		regex.Compile("\\[.+?\\]");
		return regex.Sub(source, "", true);

}

}
