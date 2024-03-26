using System;
using System.Collections.Generic;
using Unigine;
using Unigine.Plugins;

[Component(PropertyGuid = "bb02aec7dea41d7acf6d0e91a4ef76bc9e42e2eb")]
public class PCInput : VRBaseInput
{
	[ShowInEditor]
	[ParameterSlider(Title = "Throw Force", Min = 0.0f, Max = 10.0f)]
	private float throwForce = 7.0f;

	public enum KEYS : byte
	{
		ESC = 0,
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		F10,
		F11,
		F12,
		PRINTSCREEN,
		SCROLL_LOCK,
		PAUSE,
		BACK_QUOTE,
		DIGIT_1,
		DIGIT_2,
		DIGIT_3,
		DIGIT_4,
		DIGIT_5,
		DIGIT_6,
		DIGIT_7,
		DIGIT_8,
		DIGIT_9,
		DIGIT_0,
		MINUS,
		EQUALS,
		BACKSPACE,
		TAB,
		Q,
		W,
		E,
		R,
		T,
		Y,
		U,
		I,
		O,
		P,
		LEFT_BRACKET,
		RIGHT_BRACKET,
		ENTER,
		CAPS_LOCK,
		A,
		S,
		D,
		F,
		G,
		H,
		J,
		K,
		L,
		SEMICOLON,
		QUOTE,
		BACK_SLASH,
		LEFT_SHIFT,
		LESS,
		Z,
		X,
		C,
		V,
		B,
		N,
		M,
		COMMA,
		DOT,
		SLASH,
		RIGHT_SHIFT,
		LEFT_CTRL,
		LEFT_CMD,
		LEFT_ALT,
		SPACE,
		RIGHT_ALT,
		RIGHT_CMD,
		MENU,
		RIGHT_CTRL,
		INSERT,
		DELETE,
		HOME,
		END,
		PGUP,
		PGDOWN,
		UP,
		LEFT,
		DOWN,
		RIGHT,
		NUM_LOCK,
		NUMPAD_DIVIDE,
		NUMPAD_MULTIPLY,
		NUMPAD_MINUS,
		NUMPAD_DIGIT_7,
		NUMPAD_DIGIT_8,
		NUMPAD_DIGIT_9,
		NUMPAD_PLUS,
		NUMPAD_DIGIT_4,
		NUMPAD_DIGIT_5,
		NUMPAD_DIGIT_6,
		NUMPAD_DIGIT_1,
		NUMPAD_DIGIT_2,
		NUMPAD_DIGIT_3,
		NUMPAD_ENTER,
		NUMPAD_DIGIT_0,
		NUMPAD_DOT,
	}

	public enum MOUSE_BUTTONS : byte
	{
		LEFT = 0,
		MIDDLE,
		RIGHT,
		DCLICK,
		AUX_0,
		AUX_1,
		AUX_2,
		AUX_3,
	}

	public enum MOUSE_WHEEL_AXES
	{ 
		VERTICAl = 0,
		HORIZONTAL,
	}

	public enum Counts : byte
	{
		KEYS = 105,
		MOUSE_BUTTONS = 8,
		MOUSE_WHEEL_AXES = 2,
	}

	private static Input.KEY[] pcKeys = null;
	private static Input.MOUSE_BUTTON[] pcMouseButtons = null;

	public override bool HeadPositionLockInterface
	{
		get => HeadPositionLock;
		set => HeadPositionLock = value;
	}

	public override bool HeadRotationLockInterface
	{
		get => HeadRotationLock;
		set => HeadRotationLock = value;
	}

	static public bool HeadPositionLock { get { return false; } set { } }

	static public bool HeadRotationLock { get { return false; } set { } }

	static public bool IsLoaded => true;

	static private PCInput instance = null;

	private List<Gui> worldGuiInstances = new List<Gui>();

	public PCInput() : base()
	{
		if (instance != null)
			return;

		instance = this;

		pcKeys = new Input.KEY[(int)Counts.KEYS];
		pcKeys[(int)KEYS.ESC]				= Input.KEY.ESC;
		pcKeys[(int)KEYS.F1]				= Input.KEY.F1;
		pcKeys[(int)KEYS.F2]				= Input.KEY.F2;
		pcKeys[(int)KEYS.F3]				= Input.KEY.F3;
		pcKeys[(int)KEYS.F4]				= Input.KEY.F4;
		pcKeys[(int)KEYS.F5]				= Input.KEY.F5;
		pcKeys[(int)KEYS.F6]				= Input.KEY.F6;
		pcKeys[(int)KEYS.F7]				= Input.KEY.F7;
		pcKeys[(int)KEYS.F8]				= Input.KEY.F8;
		pcKeys[(int)KEYS.F9]				= Input.KEY.F9;
		pcKeys[(int)KEYS.F10]				= Input.KEY.F10;
		pcKeys[(int)KEYS.F11]				= Input.KEY.F11;
		pcKeys[(int)KEYS.F12]				= Input.KEY.F12;
		pcKeys[(int)KEYS.PRINTSCREEN]		= Input.KEY.PRINTSCREEN;
		pcKeys[(int)KEYS.SCROLL_LOCK]		= Input.KEY.SCROLL_LOCK;
		pcKeys[(int)KEYS.PAUSE]				= Input.KEY.PAUSE;
		pcKeys[(int)KEYS.BACK_QUOTE]		= Input.KEY.BACK_QUOTE;
		pcKeys[(int)KEYS.DIGIT_1]			= Input.KEY.DIGIT_1;
		pcKeys[(int)KEYS.DIGIT_2]			= Input.KEY.DIGIT_2;
		pcKeys[(int)KEYS.DIGIT_3]			= Input.KEY.DIGIT_3;
		pcKeys[(int)KEYS.DIGIT_4]			= Input.KEY.DIGIT_4;
		pcKeys[(int)KEYS.DIGIT_5]			= Input.KEY.DIGIT_5;
		pcKeys[(int)KEYS.DIGIT_6]			= Input.KEY.DIGIT_6;
		pcKeys[(int)KEYS.DIGIT_7]			= Input.KEY.DIGIT_7;
		pcKeys[(int)KEYS.DIGIT_8]			= Input.KEY.DIGIT_8;
		pcKeys[(int)KEYS.DIGIT_9]			= Input.KEY.DIGIT_9;
		pcKeys[(int)KEYS.DIGIT_0]			= Input.KEY.DIGIT_0;
		pcKeys[(int)KEYS.MINUS]				= Input.KEY.MINUS;
		pcKeys[(int)KEYS.EQUALS]			= Input.KEY.EQUALS;
		pcKeys[(int)KEYS.BACKSPACE]			= Input.KEY.BACKSPACE;
		pcKeys[(int)KEYS.TAB]				= Input.KEY.TAB;
		pcKeys[(int)KEYS.Q]					= Input.KEY.Q;
		pcKeys[(int)KEYS.W]					= Input.KEY.W;
		pcKeys[(int)KEYS.E]					= Input.KEY.E;
		pcKeys[(int)KEYS.R]					= Input.KEY.R;
		pcKeys[(int)KEYS.T]					= Input.KEY.T;
		pcKeys[(int)KEYS.Y]					= Input.KEY.Y;
		pcKeys[(int)KEYS.U]					= Input.KEY.U;
		pcKeys[(int)KEYS.I]					= Input.KEY.I;
		pcKeys[(int)KEYS.O]					= Input.KEY.O;
		pcKeys[(int)KEYS.P]					= Input.KEY.P;
		pcKeys[(int)KEYS.LEFT_BRACKET]		= Input.KEY.LEFT_BRACKET;
		pcKeys[(int)KEYS.RIGHT_BRACKET]		= Input.KEY.RIGHT_BRACKET;
		pcKeys[(int)KEYS.ENTER]				= Input.KEY.ENTER;
		pcKeys[(int)KEYS.CAPS_LOCK]			= Input.KEY.CAPS_LOCK;
		pcKeys[(int)KEYS.A]					= Input.KEY.A;
		pcKeys[(int)KEYS.S]					= Input.KEY.S;
		pcKeys[(int)KEYS.D]					= Input.KEY.D;
		pcKeys[(int)KEYS.F]					= Input.KEY.F;
		pcKeys[(int)KEYS.G]					= Input.KEY.G;
		pcKeys[(int)KEYS.H]					= Input.KEY.H;
		pcKeys[(int)KEYS.J]					= Input.KEY.J;
		pcKeys[(int)KEYS.K]					= Input.KEY.K;
		pcKeys[(int)KEYS.L]					= Input.KEY.L;
		pcKeys[(int)KEYS.SEMICOLON]			= Input.KEY.SEMICOLON;
		pcKeys[(int)KEYS.QUOTE]				= Input.KEY.QUOTE;
		pcKeys[(int)KEYS.BACK_SLASH]		= Input.KEY.BACK_SLASH;
		pcKeys[(int)KEYS.LEFT_SHIFT]		= Input.KEY.LEFT_SHIFT;
		pcKeys[(int)KEYS.LESS]				= Input.KEY.LESS;
		pcKeys[(int)KEYS.Z]					= Input.KEY.Z;
		pcKeys[(int)KEYS.X]					= Input.KEY.X;
		pcKeys[(int)KEYS.C]					= Input.KEY.C;
		pcKeys[(int)KEYS.V]					= Input.KEY.V;
		pcKeys[(int)KEYS.B]					= Input.KEY.B;
		pcKeys[(int)KEYS.N]					= Input.KEY.N;
		pcKeys[(int)KEYS.M]					= Input.KEY.M;
		pcKeys[(int)KEYS.COMMA]				= Input.KEY.COMMA;
		pcKeys[(int)KEYS.DOT]				= Input.KEY.DOT;
		pcKeys[(int)KEYS.SLASH]				= Input.KEY.SLASH;
		pcKeys[(int)KEYS.RIGHT_SHIFT]		= Input.KEY.RIGHT_SHIFT;
		pcKeys[(int)KEYS.LEFT_CTRL]			= Input.KEY.LEFT_CTRL;
		pcKeys[(int)KEYS.LEFT_CMD]			= Input.KEY.LEFT_CMD;
		pcKeys[(int)KEYS.LEFT_ALT]			= Input.KEY.LEFT_ALT;
		pcKeys[(int)KEYS.SPACE]				= Input.KEY.SPACE;
		pcKeys[(int)KEYS.RIGHT_ALT]			= Input.KEY.RIGHT_ALT;
		pcKeys[(int)KEYS.RIGHT_CMD]			= Input.KEY.RIGHT_CMD;
		pcKeys[(int)KEYS.MENU]				= Input.KEY.MENU;
		pcKeys[(int)KEYS.RIGHT_CTRL]		= Input.KEY.RIGHT_CTRL;
		pcKeys[(int)KEYS.INSERT]			= Input.KEY.INSERT;
		pcKeys[(int)KEYS.DELETE]			= Input.KEY.DELETE;
		pcKeys[(int)KEYS.HOME]				= Input.KEY.HOME;
		pcKeys[(int)KEYS.END]				= Input.KEY.END;
		pcKeys[(int)KEYS.PGUP]				= Input.KEY.PGUP;
		pcKeys[(int)KEYS.PGDOWN]			= Input.KEY.PGDOWN;
		pcKeys[(int)KEYS.UP]				= Input.KEY.UP;
		pcKeys[(int)KEYS.LEFT]				= Input.KEY.LEFT;
		pcKeys[(int)KEYS.DOWN]				= Input.KEY.DOWN;
		pcKeys[(int)KEYS.RIGHT]				= Input.KEY.RIGHT;
		pcKeys[(int)KEYS.NUM_LOCK]			= Input.KEY.NUM_LOCK;
		pcKeys[(int)KEYS.NUMPAD_DIVIDE]		= Input.KEY.NUMPAD_DIVIDE;
		pcKeys[(int)KEYS.NUMPAD_MULTIPLY]	= Input.KEY.NUMPAD_MULTIPLY;
		pcKeys[(int)KEYS.NUMPAD_MINUS]		= Input.KEY.NUMPAD_MINUS;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_7]	= Input.KEY.NUMPAD_DIGIT_7;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_8]	= Input.KEY.NUMPAD_DIGIT_8;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_9]	= Input.KEY.NUMPAD_DIGIT_9;
		pcKeys[(int)KEYS.NUMPAD_PLUS]		= Input.KEY.NUMPAD_PLUS;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_4]	= Input.KEY.NUMPAD_DIGIT_4;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_5]	= Input.KEY.NUMPAD_DIGIT_5;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_6]	= Input.KEY.NUMPAD_DIGIT_6;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_1]	= Input.KEY.NUMPAD_DIGIT_1;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_2]	= Input.KEY.NUMPAD_DIGIT_2;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_3]	= Input.KEY.NUMPAD_DIGIT_3;
		pcKeys[(int)KEYS.NUMPAD_ENTER]		= Input.KEY.NUMPAD_ENTER;
		pcKeys[(int)KEYS.NUMPAD_DIGIT_0]	= Input.KEY.NUMPAD_DIGIT_0;
		pcKeys[(int)KEYS.NUMPAD_DOT]		= Input.KEY.NUMPAD_DOT;

		pcMouseButtons = new Input.MOUSE_BUTTON[(int)Counts.MOUSE_BUTTONS];
		pcMouseButtons[(int)MOUSE_BUTTONS.LEFT]		= Input.MOUSE_BUTTON.LEFT;
		pcMouseButtons[(int)MOUSE_BUTTONS.MIDDLE]	= Input.MOUSE_BUTTON.MIDDLE;
		pcMouseButtons[(int)MOUSE_BUTTONS.RIGHT]	= Input.MOUSE_BUTTON.RIGHT;
		pcMouseButtons[(int)MOUSE_BUTTONS.DCLICK]	= Input.MOUSE_BUTTON.DCLICK;
		pcMouseButtons[(int)MOUSE_BUTTONS.AUX_0]	= Input.MOUSE_BUTTON.AUX_0;
		pcMouseButtons[(int)MOUSE_BUTTONS.AUX_1]	= Input.MOUSE_BUTTON.AUX_1;
		pcMouseButtons[(int)MOUSE_BUTTONS.AUX_2]	= Input.MOUSE_BUTTON.AUX_2;
		pcMouseButtons[(int)MOUSE_BUTTONS.AUX_3]	= Input.MOUSE_BUTTON.AUX_3;

		Gui.GetWorldGuiInstances(worldGuiInstances);
	}

	private void Update()
	{
		if (Input.MouseGrab == false && InputSystem.Current == this)
		{
			foreach (Gui gui in worldGuiInstances)
			{
				gui.RemoveFocus();
			}
		}
	}

	static public bool IsKeyDown(KEYS key)
	{
		return Input.MouseGrab && Input.IsKeyDown(pcKeys[(int)key]);
	}

	static public bool IsKeyPress(KEYS key)
	{
		return Input.MouseGrab && Input.IsKeyPressed(pcKeys[(int)key]);
	}

	static public bool IsKeyUp(KEYS key)
	{
		return Input.MouseGrab && Input.IsKeyUp(pcKeys[(int)key]);
	}

	static public bool IsMouseButtonDown(MOUSE_BUTTONS button)
	{
		return Input.MouseGrab && Input.IsMouseButtonDown(pcMouseButtons[(int)button]);
	}

	static public bool IsMouseButtonPress(MOUSE_BUTTONS button)
	{
		return Input.MouseGrab && Input.IsMouseButtonPressed(pcMouseButtons[(int)button]);
	}

	static public bool IsMouseButtonUp(MOUSE_BUTTONS button)
	{
		return Input.MouseGrab && Input.IsMouseButtonUp(pcMouseButtons[(int)button]);
	}

	static public vec3 GetAngularVelocity(InputSystem.VRDevice device)
	{
		if (!IsLoaded)
			return vec3.ZERO;

		float x = Game.GetRandomFloat(-1.0f, 1.0f);
		float y = Game.GetRandomFloat(-1.0f, 1.0f);
		float z = Game.GetRandomFloat(-1.0f, 1.0f);

		return new vec3(x, y, z) * instance.throwForce;
	}

	static public mat4 GetTransform(InputSystem.VRDevice device)
	{
		if (!IsLoaded)
			return mat4.IDENTITY;

		return mat4.IDENTITY;
	}

	static public vec3 GetLinearVelocity(InputSystem.VRDevice device)
	{
		if (!IsLoaded || !VRPlayer.LastPlayer)
			return vec3.ZERO;

		// in local transform
		return vec3.DOWN * instance.throwForce;
	}

	static public float GetAxis(MOUSE_WHEEL_AXES axis)
	{
		if (!IsLoaded)
			return 0.0f;

		switch (axis)
		{
			case MOUSE_WHEEL_AXES.VERTICAl: return Input.MouseWheel;
			case MOUSE_WHEEL_AXES.HORIZONTAL: return Input.MouseWheelHorizontal;
			default: break;
		}

		return 0.0f;
	}

	public override mat4 GetTransformInterface(InputSystem.VRDevice device)
	{
		return GetTransform(device);
	}

	public override bool IsDeviceConnectedInterface(InputSystem.VRDevice device)
	{
		if (device == InputSystem.VRDevice.PC_HEAD || device == InputSystem.VRDevice.PC_HAND)
			return true;
		else
			return false;
	}

	public override bool IsTransformValidInterface(InputSystem.VRDevice device)
	{
		if (device == InputSystem.VRDevice.PC_HEAD || device == InputSystem.VRDevice.PC_HAND)
			return true;
		else
			return false;
	}

	public override vec3 GetLinearVelocityInterface(InputSystem.VRDevice device)
	{
		return GetLinearVelocity(device);
	}

	public override vec3 GetAngularVelocityInterface(InputSystem.VRDevice device)
	{
		return GetAngularVelocity(device);
	}

	public override void SetControllerVibrationInterface(InputSystem.VRDevice device, ushort duration, float amplitude)
	{
		return;
	}

	public override bool IsLoadedInterface => true;
}
