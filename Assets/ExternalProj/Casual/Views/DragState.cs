using UnityEngine;
using System.Collections;

/**
  * List of possible building states
  */
public enum DragState {
	PLACING,			// Building being initially placed on the map
	PLACING_INVALID,	// Building being initially placed on the map, but currently in a place where it cannot be built
	MOVING,				// Building is built but being moved
	PLACED				// Building is placed at valid position on map
}

