//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
#define MOBILE
#endif

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Input field makes it possible to enter custom information within the UI.
/// </summary>

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	public enum InputType
	{
		Standard,
		AutoCorrect,
		Password,
	}

	public enum Validation
	{
		None,
		Integer,
		Float,
		Alphanumeric,
		Username,
		Name,
	}

	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7,
	}

	public delegate char OnValidate (string text, int charIndex, char addedChar);

	/// <summary>
	/// Currently active input field. Only valid during callbacks.
	/// </summary>

	static public UIInput current;

	/// <summary>
	/// Currently selected input field, if any.
	/// </summary>

	static public UIInput selection;

	/// <summary>
	/// Text label used to display the input's value.
	/// </summary>

	public UILabel label;

	/// <summary>
	/// Type of data expected by the input field.
	/// </summary>

	public InputType inputType = InputType.Standard;

	/// <summary>
	/// Keyboard type applies to mobile keyboards that get shown.
	/// </summary>

	public KeyboardType keyboardType = KeyboardType.Default;

	/// <summary>
	/// What kind of validation to use with the input field's data.
	/// </summary>

	public Validation validation = Validation.None;

	/// <summary>
	/// Maximum number of characters allowed before input no longer works.
	/// </summary>

	public int characterLimit = 0; 

	/// <summary>
	/// Field in player prefs used to automatically save the value.
	/// </summary>

	public string savedAs;

	/// <summary>
	/// Object to select when Tab key gets pressed.
	/// </summary>

	public GameObject selectOnTab;

	/// <summary>
	/// Color of the label when the input field has focus.
	/// </summary>

	public Color activeTextColor = Color.white;

	/// <summary>
	/// Event delegates triggered when the input field submits its data.
	/// </summary>

	public List<EventDelegate> onSubmit = new List<EventDelegate>();

	/// <summary>
	/// Custom validation callback.
	/// </summary>

	public OnValidate onValidate;

	/// <summary>
	/// Input field's value.
	/// </summary>

	[SerializeField][HideInInspector] protected string mValue;

	protected string mDefaultText = "";
	protected Color mDefaultColor = Color.white;
	protected float mPosition = 0f;
	protected bool mDoInit = true;
	protected UIWidget.Pivot mPivot = UIWidget.Pivot.TopLeft;
	
	static protected int mDrawStart = 0;
	static protected int mDrawEnd = 0;

#if MOBILE
	static protected TouchScreenKeyboard mKeyboard;
#else
	static protected string mLastIME = "";
	static protected TextEditor mEditor = null;
#endif

	/// <summary>
	/// Default text used by the input's label.
	/// </summary>

	public string defaultText { get { return mDefaultText; } set { mDefaultText = value; } }

	[System.Obsolete("Use UIInput.value instead")]
	public string text { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Input field's current text value.
	/// </summary>

	public string value
	{
		get
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return "";
#endif
			if (mDoInit) Init();
#if MOBILE
			if (isSelected && mKeyboard != null && mKeyboard.active) return mKeyboard.text;
#else
			if (isSelected && mEditor != null) return mEditor.content.text;
#endif
			return mValue;
		}
		set
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (mDoInit) Init();
#if MOBILE
			if (isSelected && mKeyboard != null)
				mKeyboard.text = value;

			if (this.value != value)
			{
				mValue = value;
				if (isSelected && mKeyboard != null) mKeyboard.text = value;
				SaveToPlayerPrefs(mValue);
				UpdateLabel();
			}
#else
			if (isSelected && mEditor != null)
			{
				if (mEditor.content.text != value)
				{
					mEditor.content.text = value;
					UpdateLabel();
					return;
				}
			}

			if (this.value != value)
			{
				mValue = value;

				if (isSelected && mEditor != null)
				{
					mEditor.content.text = value;
					mEditor.OnLostFocus();
					mEditor.OnFocus();
					mEditor.MoveTextEnd();
				}
				SaveToPlayerPrefs(mValue);
				UpdateLabel();
			}
#endif
		}
	}

	/// <summary>
	/// Whether the input field needs to draw an ASCII cursor.
	/// </summary>

#if MOBILE
	protected bool needsTextCursor { get { return (isSelected && mKeyboard == null); } }
#else
	protected bool needsTextCursor { get { return isSelected; } }
#endif

	[System.Obsolete("Use UIInput.isSelected instead")]
	public bool selected { get { return isSelected; } set { isSelected = value; } }

	/// <summary>
	/// Whether the input is currently selected.
	/// </summary>

	public bool isSelected
	{
		get
		{
			return selection == this;
		}
		set
		{
			if (!value) { if (isSelected) UICamera.selectedObject = null; }
			else UICamera.selectedObject = gameObject;
		}
	}

	/// <summary>
	/// Current position of the cursor.
	/// </summary>


	protected int cursorPosition { get { return value.Length; } }


	/// <summary>
	/// Automatically set the value by loading it from player prefs if possible.
	/// </summary>

	void Start ()
	{
		if (string.IsNullOrEmpty(mValue))
		{
			if (!string.IsNullOrEmpty(savedAs) && PlayerPrefs.HasKey(savedAs))
				value = PlayerPrefs.GetString(savedAs);
		}
		else value = mValue;
	}

	/// <summary>
	/// Labels used for input shouldn't support rich text.
	/// </summary>

	protected void Init ()
	{
		if (mDoInit && label != null)
		{
			mDoInit = false;
			mDefaultText = label.text;
			mDefaultColor = label.color;
			label.supportEncoding = false;
			mPivot = label.pivot;
			mPosition = label.cachedTransform.localPosition.x;
			UpdateLabel();
		}
	}

	/// <summary>
	/// Save the specified value to player prefs.
	/// </summary>

	protected void SaveToPlayerPrefs (string val)
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			if (string.IsNullOrEmpty(val)) PlayerPrefs.DeleteKey(savedAs);
			else PlayerPrefs.SetString(savedAs, val);
		}
	}

	/// <summary>
	/// Selection event, sent by the EventSystem.
	/// </summary>

	protected virtual void OnSelect (bool isSelected)
	{
		if (isSelected) OnSelectEvent();
		else OnDeselectEvent();
	}

	/// <summary>
	/// Notification of the input field gaining selection.
	/// </summary>

	protected void OnSelectEvent ()
	{
		
	}

	/// <summary>
	/// Notification of the input field losing selection.
	/// </summary>

	protected void OnDeselectEvent ()
	{
	
	}



	void Update ()
	{

	}

	/// <summary>
	/// Unfortunately Unity 4.3 and earlier doesn't offer a way to properly process events outside of OnGUI.
	/// </summary>

	void OnGUI ()
	{
		if (isSelected && Event.current.rawType == EventType.KeyDown)
			ProcessEvent(Event.current);
	}

	/// <summary>
	/// Handle the specified event.
	/// </summary>

	bool ProcessEvent (Event ev)
	{
		
		return false;
	}


	/// <summary>
	/// Submit the input field's text.
	/// </summary>

	protected void Submit ()
	{
		if (NGUITools.IsActive(this))
		{
			current = this;
			mValue = value;
			EventDelegate.Execute(onSubmit);
			SaveToPlayerPrefs(mValue);
			current = null;
		}
	}

#if !MOBILE
	/// <summary>
	/// Append the specified text to the end of the current.
	/// </summary>

	protected virtual void Append (string input)
	{
	
	}
#endif

	/// <summary>
	/// Update the visual text label.
	/// </summary>

	protected void UpdateLabel ()
	{
		if (label != null)
		{
			if (mDoInit) Init();
			bool selected = isSelected;
			string fullText = value;
			bool isEmpty = string.IsNullOrEmpty(fullText);
			string processed;

			if (isEmpty)
			{
				processed = selected ? (needsTextCursor ? "|" : "") : mDefaultText;
				RestoreLabelPivot();
			}
			else
			{
				if (inputType == InputType.Password)
				{
					processed = "";
					for (int i = 0, imax = fullText.Length; i < imax; ++i) processed += "*";
				}
				else processed = fullText;

				// Start with text leading up to the selection
				int selPos = selected ? Mathf.Min(processed.Length, cursorPosition) : 0;
				string left = processed.Substring(0, selPos);
					
				if (selected)
				{
					// Append the composition string and the cursor character
					left += Input.compositionString;
					if (needsTextCursor) left += "|";
				}
				
				// Append the text from the selection onwards
				processed = left + processed.Substring(selPos, processed.Length - selPos);

				if (label.overflowMethod == UILabel.Overflow.ClampContent)
				{
					// Determine what will actually fit into the given line
					if (selected)
					{
						if (mDrawEnd == 0) mDrawEnd = selPos;

						// Offset required in order to print the part leading up to the cursor
						string visible = processed.Substring(0, Mathf.Min(mDrawEnd, processed.Length));
						int leftMargin = label.CalculateOffsetToFit(visible);

						// The cursor is no longer within bounds
						if (selPos < leftMargin || selPos >= mDrawEnd)
						{
							leftMargin = label.CalculateOffsetToFit(left);
							mDrawStart = leftMargin;
							mDrawEnd = left.Length;
						}
						else if (leftMargin != mDrawStart)
						{
							// The left margin shifted -- happens when deleting or adding characters
							mDrawStart = leftMargin;
						}
					}

					// If the text doesn't fit, we want to change the label to use a right-hand pivot point
					if (mDrawStart != 0)
					{
						processed = processed.Substring(mDrawStart, processed.Length - mDrawStart);
						if (mPivot == UIWidget.Pivot.Left) label.pivot = UIWidget.Pivot.Right;
						else if (mPivot == UIWidget.Pivot.TopLeft) label.pivot = UIWidget.Pivot.TopRight;
						else if (mPivot == UIWidget.Pivot.BottomLeft) label.pivot = UIWidget.Pivot.BottomRight;
					}
					else RestoreLabelPivot();
				}
				else RestoreLabelPivot();
			}

			label.text = processed;
			label.color = (isEmpty && !selected) ? mDefaultColor : activeTextColor;
		}
	}

	/// <summary>
	/// Restore the input label's pivot point.
	/// </summary>

	protected void RestoreLabelPivot ()
	{
		if (label != null && label.pivot != mPivot)
			label.pivot = mPivot;
	}

	/// <summary>
	/// Validate the specified input.
	/// </summary>

	protected char Validate (string text, int pos, char ch)
	{
		// Validation is disabled
		if (validation == Validation.None || !enabled) return ch;

		if (validation == Validation.Integer)
		{
			// Integer number validation
			if (ch >= '0' && ch <= '9') return ch;
			if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
		}
		else if (validation == Validation.Float)
		{
			// Floating-point number
			if (ch >= '0' && ch <= '9') return ch;
			if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
			if (ch == '.' && !text.Contains(".")) return ch;
		}
		else if (validation == Validation.Alphanumeric)
		{
			// All alphanumeric characters
			if (ch >= 'A' && ch <= 'Z') return ch;
			if (ch >= 'a' && ch <= 'z') return ch;
			if (ch >= '0' && ch <= '9') return ch;
		}
		else if (validation == Validation.Username)
		{
			// Lowercase and numbers
			if (ch >= 'A' && ch <= 'Z') return (char)(ch - 'A' + 'a');
			if (ch >= 'a' && ch <= 'z') return ch;
			if (ch >= '0' && ch <= '9') return ch;
		}
		else if (validation == Validation.Name)
		{
			char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
			char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';

			if (ch >= 'a' && ch <= 'z')
			{
				// Space followed by a letter -- make sure it's capitalized
				if (lastChar == ' ') return (char)(ch - 'a' + 'A');
				return ch;
			}
			else if (ch >= 'A' && ch <= 'Z')
			{
				// Uppercase letters are only allowed after spaces (and apostrophes)
				if (lastChar != ' ' && lastChar != '\'') return (char)(ch - 'A' + 'a');
				return ch;
			}
			else if (ch == '\'')
			{
				// Don't allow more than one apostrophe
				if (lastChar != ' ' && lastChar != '\'' && nextChar != '\'' && !text.Contains("'")) return ch;
			}
			else if (ch == ' ')
			{
				// Don't allow more than one space in a row
				if (lastChar != ' ' && lastChar != '\'' && nextChar != ' ' && nextChar != '\'') return ch;
			}
		}
		return (char)0;
	}
}
