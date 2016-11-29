using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScheduledPopup {
	
	protected UIGamePanel panel;
	protected long delay;
	protected long DEFAULT_DELAY_IN_MSEC = 10;

	public ScheduledPopup(UIGamePanel panel) {
		Initialize(panel, DEFAULT_DELAY_IN_MSEC);
	}

	public ScheduledPopup(UIGamePanel panel, long delay) {
		Initialize (panel, delay);
	}

	private void Initialize(UIGamePanel panel, long delay) {
		this.panel = panel;
		this.delay = delay;
	}
	
	public UIGamePanel GetPanel() {
		return panel;
	}
	
	public float GetDelay() {
		return (float)delay/1000;
	}

}
