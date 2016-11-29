using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface IMazeMapGeneratorAlgo {
    MazeMap GenerateMaze (int width, int height);
}
