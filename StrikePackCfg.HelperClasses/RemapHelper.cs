using System.Collections.Generic;
using System.Linq;
using StrikePackCfg.Properties;

namespace StrikePackCfg.HelperClasses;

public static class RemapHelper
{
	public static void SwapButton(ref byte[] assignments, byte src, byte dst)
	{
		byte b = assignments[src];
		byte b2 = assignments[dst];
		if (src == dst)
		{
			if (assignments[b] != src)
			{
				b2 = assignments[b];
				assignments[b2] = assignments[src];
				assignments[b] = b2;
				assignments[src] = dst;
			}
			else
			{
				assignments[b] = b2;
				assignments[src] = dst;
			}
		}
		else
		{
			assignments[src] = b2;
			assignments[dst] = b;
		}
	}

	public static IDictionary<byte, string> GetXboxOneButtons(bool includeSticks = false)
	{
		List<KeyValuePair<byte, string>> list = new List<KeyValuePair<byte, string>>
		{
			new KeyValuePair<byte, string>(0, XB1Buttons.Home),
			new KeyValuePair<byte, string>(1, XB1Buttons.View),
			new KeyValuePair<byte, string>(2, XB1Buttons.Menu),
			new KeyValuePair<byte, string>(3, XB1Buttons.RB),
			new KeyValuePair<byte, string>(4, XB1Buttons.RT),
			new KeyValuePair<byte, string>(5, XB1Buttons.RS),
			new KeyValuePair<byte, string>(6, XB1Buttons.LB),
			new KeyValuePair<byte, string>(7, XB1Buttons.LT),
			new KeyValuePair<byte, string>(8, XB1Buttons.LS),
			new KeyValuePair<byte, string>(13, XB1Buttons.Up),
			new KeyValuePair<byte, string>(14, XB1Buttons.Down),
			new KeyValuePair<byte, string>(15, XB1Buttons.Left),
			new KeyValuePair<byte, string>(16, XB1Buttons.Right),
			new KeyValuePair<byte, string>(17, XB1Buttons.Y),
			new KeyValuePair<byte, string>(18, XB1Buttons.B),
			new KeyValuePair<byte, string>(19, XB1Buttons.A),
			new KeyValuePair<byte, string>(20, XB1Buttons.X)
		};
		if (includeSticks)
		{
			list.AddRange(GetXboxOneSticks());
		}
		list.Sort((KeyValuePair<byte, string> k1, KeyValuePair<byte, string> k2) => k1.Key.CompareTo(k2.Key));
		return list.ToDictionary((KeyValuePair<byte, string> kvp) => kvp.Key, (KeyValuePair<byte, string> kvp) => kvp.Value);
	}

	public static IDictionary<byte, string> GetXboxOneSticks()
	{
		return new Dictionary<byte, string>
		{
			{
				9,
				XB1Buttons.RX
			},
			{
				10,
				XB1Buttons.RY
			},
			{
				11,
				XB1Buttons.LX
			},
			{
				12,
				XB1Buttons.LY
			}
		};
	}

	public static IDictionary<ushort, string> GetXboxOnePaddles()
	{
		return new Dictionary<ushort, string>
		{
			{
				0,
				XB1Buttons.None
			},
			{
				16,
				XB1Buttons.A
			},
			{
				32,
				XB1Buttons.B
			},
			{
				64,
				XB1Buttons.X
			},
			{
				128,
				XB1Buttons.Y
			},
			{
				256,
				XB1Buttons.Up
			},
			{
				512,
				XB1Buttons.Down
			},
			{
				1024,
				XB1Buttons.Left
			},
			{
				2048,
				XB1Buttons.Right
			},
			{
				2,
				XB1Buttons.RT
			},
			{
				8192,
				XB1Buttons.RB
			},
			{
				32768,
				XB1Buttons.RS
			},
			{
				1,
				XB1Buttons.LT
			},
			{
				4096,
				XB1Buttons.LB
			},
			{
				16384,
				XB1Buttons.LS
			},
			{
				4,
				XB1Buttons.View
			},
			{
				8,
				XB1Buttons.Menu
			}
		};
	}

	public static IDictionary<byte, string> GetSwitchButtons(bool includeSticks = false)
	{
		List<KeyValuePair<byte, string>> list = new List<KeyValuePair<byte, string>>
		{
			new KeyValuePair<byte, string>(0, SwitchButtons.Home),
			new KeyValuePair<byte, string>(1, SwitchButtons.Minus),
			new KeyValuePair<byte, string>(2, SwitchButtons.Plus),
			new KeyValuePair<byte, string>(3, SwitchButtons.R),
			new KeyValuePair<byte, string>(4, SwitchButtons.ZR),
			new KeyValuePair<byte, string>(5, SwitchButtons.RS),
			new KeyValuePair<byte, string>(6, SwitchButtons.L),
			new KeyValuePair<byte, string>(7, SwitchButtons.ZL),
			new KeyValuePair<byte, string>(8, SwitchButtons.LS),
			new KeyValuePair<byte, string>(13, SwitchButtons.Up),
			new KeyValuePair<byte, string>(14, SwitchButtons.Down),
			new KeyValuePair<byte, string>(15, SwitchButtons.Left),
			new KeyValuePair<byte, string>(16, SwitchButtons.Right),
			new KeyValuePair<byte, string>(17, SwitchButtons.X),
			new KeyValuePair<byte, string>(18, SwitchButtons.A),
			new KeyValuePair<byte, string>(19, SwitchButtons.B),
			new KeyValuePair<byte, string>(20, SwitchButtons.Y)
		};
		if (includeSticks)
		{
			list.AddRange(GetSwitchSticks());
		}
		list.Sort((KeyValuePair<byte, string> k1, KeyValuePair<byte, string> k2) => k1.Key.CompareTo(k2.Key));
		return list.ToDictionary((KeyValuePair<byte, string> kvp) => kvp.Key, (KeyValuePair<byte, string> kvp) => kvp.Value);
	}

	public static IDictionary<byte, string> GetSwitchSticks()
	{
		return new Dictionary<byte, string>
		{
			{
				9,
				SwitchButtons.RX
			},
			{
				10,
				SwitchButtons.RY
			},
			{
				11,
				SwitchButtons.LX
			},
			{
				12,
				SwitchButtons.LY
			}
		};
	}

	public static IDictionary<byte, string> GetPs4Buttons(bool includeSticks = false)
	{
		List<KeyValuePair<byte, string>> list = new List<KeyValuePair<byte, string>>
		{
			new KeyValuePair<byte, string>(1, PS4Buttons.Share),
			new KeyValuePair<byte, string>(2, PS4Buttons.Options),
			new KeyValuePair<byte, string>(3, PS4Buttons.R1),
			new KeyValuePair<byte, string>(4, PS4Buttons.R2),
			new KeyValuePair<byte, string>(5, PS4Buttons.R3),
			new KeyValuePair<byte, string>(6, PS4Buttons.L1),
			new KeyValuePair<byte, string>(7, PS4Buttons.L2),
			new KeyValuePair<byte, string>(8, PS4Buttons.L3),
			new KeyValuePair<byte, string>(13, PS4Buttons.Up),
			new KeyValuePair<byte, string>(14, PS4Buttons.Down),
			new KeyValuePair<byte, string>(15, PS4Buttons.Left),
			new KeyValuePair<byte, string>(16, PS4Buttons.Right),
			new KeyValuePair<byte, string>(17, PS4Buttons.Triangle),
			new KeyValuePair<byte, string>(18, PS4Buttons.Circle),
			new KeyValuePair<byte, string>(19, PS4Buttons.Cross),
			new KeyValuePair<byte, string>(20, PS4Buttons.Square),
			new KeyValuePair<byte, string>(27, PS4Buttons.Touch)
		};
		if (includeSticks)
		{
			list.AddRange(GetPs4Sticks());
		}
		list.Sort((KeyValuePair<byte, string> k1, KeyValuePair<byte, string> k2) => k1.Key.CompareTo(k2.Key));
		return list.ToDictionary((KeyValuePair<byte, string> kvp) => kvp.Key, (KeyValuePair<byte, string> kvp) => kvp.Value);
	}

	public static IDictionary<byte, string> GetPs4Sticks()
	{
		return new Dictionary<byte, string>
		{
			{
				9,
				PS4Buttons.RX
			},
			{
				10,
				PS4Buttons.RY
			},
			{
				11,
				PS4Buttons.LX
			},
			{
				12,
				PS4Buttons.LY
			}
		};
	}

	public static IDictionary<uint, string> GetPs4Paddles()
	{
		return new Dictionary<uint, string>
		{
			{
				0u,
				PS4Buttons.None
			},
			{
				32u,
				PS4Buttons.Cross
			},
			{
				64u,
				PS4Buttons.Circle
			},
			{
				16u,
				PS4Buttons.Square
			},
			{
				128u,
				PS4Buttons.Triangle
			},
			{
				1u,
				PS4Buttons.Up
			},
			{
				2u,
				PS4Buttons.Down
			},
			{
				4u,
				PS4Buttons.Left
			},
			{
				8u,
				PS4Buttons.Right
			},
			{
				512u,
				PS4Buttons.R1
			},
			{
				2048u,
				PS4Buttons.R2
			},
			{
				32768u,
				PS4Buttons.R3
			},
			{
				256u,
				PS4Buttons.L1
			},
			{
				1024u,
				PS4Buttons.L2
			},
			{
				16384u,
				PS4Buttons.L3
			},
			{
				4096u,
				PS4Buttons.Share
			},
			{
				8192u,
				PS4Buttons.Options
			},
			{
				65536u,
				PS4Buttons.Home
			},
			{
				131072u,
				PS4Buttons.Touch
			}
		};
	}
}
