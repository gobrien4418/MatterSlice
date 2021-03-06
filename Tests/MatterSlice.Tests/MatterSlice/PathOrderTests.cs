﻿/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using MatterSlice.ClipperLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace MatterHackers.MatterSlice.Tests
{
	[TestFixture, Category("MatterSlice.PathOrderTests")]
	public class PathOrderTests
	{
		[Test]
		public void CorrectSeamPlacement()
		{
			// coincident points return 0 angle
			{
				IntPoint p1 = new IntPoint(10, 0);
				IntPoint p2 = new IntPoint(0, 0);
				IntPoint p3 = new IntPoint(0, 0);
				Assert.IsTrue(PathOrderOptimizer.GetTurnAmount(p1, p2, p3) == 0);
			}

			// no turn returns a 0 angle
			{
				IntPoint p1 = new IntPoint(10, 0);
				IntPoint p2 = new IntPoint(0, 0);
				IntPoint p3 = new IntPoint(-10, 0);
				Assert.IsTrue(PathOrderOptimizer.GetTurnAmount(p1, p2, p3) == 0);
			}

			// 90 turn works
			{
				IntPoint p1 = new IntPoint(0, 0);
				IntPoint p2 = new IntPoint(10, 0);
				IntPoint p3 = new IntPoint(10, 10);
				Assert.AreEqual(PathOrderOptimizer.GetTurnAmount(p1, p2, p3), Math.PI / 2, .001);

				IntPoint p4 = new IntPoint(0, 10);
				IntPoint p5 = new IntPoint(0, 0);
				IntPoint p6 = new IntPoint(10, 0);
				Assert.AreEqual(PathOrderOptimizer.GetTurnAmount(p4, p5, p6), Math.PI / 2, .001);
			}

			// -90 turn works
			{
				IntPoint p1 = new IntPoint(0, 0);
				IntPoint p2 = new IntPoint(10, 0);
				IntPoint p3 = new IntPoint(10, -10);
				Assert.AreEqual(PathOrderOptimizer.GetTurnAmount(p1, p2, p3), -Math.PI / 2, .001);
			}

			// 45 turn works
			{
				IntPoint p1 = new IntPoint(0, 0);
				IntPoint p2 = new IntPoint(10, 0);
				IntPoint p3 = new IntPoint(15, 5);
				Assert.AreEqual(Math.PI / 4, PathOrderOptimizer.GetTurnAmount(p1, p2, p3), .001);

				IntPoint p4 = new IntPoint(0, 0);
				IntPoint p5 = new IntPoint(-10, 0);
				IntPoint p6 = new IntPoint(-15, -5);
				Assert.AreEqual(Math.PI / 4, PathOrderOptimizer.GetTurnAmount(p4, p5, p6), .001);
			}

			// -45 turn works
			{
				IntPoint p1 = new IntPoint(0, 0);
				IntPoint p2 = new IntPoint(10, 0);
				IntPoint p3 = new IntPoint(15, -5);
				Assert.AreEqual(-Math.PI / 4, PathOrderOptimizer.GetTurnAmount(p1, p2, p3), .001);
			}

			// find the right point wound ccw
			{
				List<IntPoint> testPoints = new List<IntPoint> { new IntPoint(0, 0), new IntPoint(10, 0), new IntPoint(9, 5), new IntPoint(10, 10), new IntPoint(0, 10) };
				int betsPoint = PathOrderOptimizer.GetBestEdgeIndex(testPoints);
				Assert.IsTrue(betsPoint == 2);
			}

			{
				List<IntPoint> testPoints = new List<IntPoint> { new IntPoint(9, 5), new IntPoint(10, 10), new IntPoint(0, 10), new IntPoint(0, 0), new IntPoint(10, 0) };
				int betsPoint = PathOrderOptimizer.GetBestEdgeIndex(testPoints);
				Assert.IsTrue(betsPoint == 0);
			}

			// find the right point wound cw
			{
				List<IntPoint> testPoints = new List<IntPoint> { new IntPoint(0, 0), new IntPoint(0, 10), new IntPoint(10, 10), new IntPoint(9, 5), new IntPoint(10, 0) };
				int betsPoint = PathOrderOptimizer.GetBestEdgeIndex(testPoints);
				Assert.IsTrue(betsPoint == 3);
			}

			{
				List<IntPoint> testPoints = new List<IntPoint> { new IntPoint(10, 0), new IntPoint(0, 0), new IntPoint(0, 10), new IntPoint(10, 10), new IntPoint(9, 5) };
				int betsPoint = PathOrderOptimizer.GetBestEdgeIndex(testPoints);
				Assert.IsTrue(betsPoint == 4);
			}
		}
	}
}