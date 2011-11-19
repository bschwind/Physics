using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics
{
    public class Intersection
    {
        public static bool TestLineLine(LineSegment line1, LineSegment line2, out Vector2 hitPoint, out bool parallel)
        {
            hitPoint = Vector2.Zero;

            Vector2 dir1 = line1.End - line1.Start;
            Vector2 perpDir1 = new Vector2(-dir1.Y, dir1.X);

            float a1 = perpDir1.X;
            float b1 = perpDir1.Y;
            float d1 = perpDir1.X * line1.Start.X + perpDir1.Y * line1.Start.Y;

            Vector2 dir2 = line2.End - line2.Start;
            Vector2 perpDir2 = new Vector2(-dir2.Y, dir2.X);

            float a2 = perpDir2.X;
            float b2 = perpDir2.Y;
            float d2 = perpDir2.X * line2.Start.X + perpDir2.Y * line2.Start.Y;

            float denom = (a1 * b2 - a2 * b1);
            if (Math.Abs(denom) < float.Epsilon)
            {
                parallel = true;
                return false;
            }
            else
            {
                hitPoint = new Vector2((b2 * d1 - b1 * d2) / denom, (a1 * d2 - a2 * d1) / denom);
                parallel = false;
                return true;
            }
        }

        //Find the closest point on a circle from the start point of the segment
        public static bool TestSegmentCircle(LineSegment line, Vector2 center, float radius, out Vector2 hitPoint)
        {
            //This shit has a problem when the line is perfectly horizontal

            hitPoint = Vector2.Zero;

            line.End -= center;
            line.Start -= center;

            float dx = line.End.X - line.Start.X;
            float dy = line.End.Y - line.Start.Y;
            float drsq = dx * dx + dy * dy;
            float D = line.Start.X * line.End.Y - line.End.X * line.Start.Y;
            float discr = (radius * radius * drsq) - (D * D);

            if (discr < 0 || (float)Math.Abs(discr) <= float.Epsilon) //The == 0 part is a hack, fix it later
            {
                return false;
            }
            else
            {
                float discrSqrt = (float)Math.Sqrt(discr);
                float quadPart1 = (float)(Math.Sign(dy) * dx * discrSqrt);
                float quadPart2 = (float)(Math.Abs(dy)*discrSqrt);
                hitPoint = new Vector2((D * dy + quadPart1) / drsq, (-D * dx + quadPart2) / drsq);
                Vector2 tempPoint = new Vector2((D * dy - quadPart1) / drsq, (-D * dx - quadPart2) / drsq);

                //If hitPoint is closer than tempPoint, use it
                if ((hitPoint - line.Start).LengthSquared() < (tempPoint - line.Start).LengthSquared())
                {
                    if (Vector2.Dot(hitPoint - line.Start, line.End - line.Start) < 0 || (hitPoint-line.Start).LengthSquared() > (line.End-line.Start).LengthSquared())
                    {
                        return false;
                    }

                    hitPoint += center;
                    return true;
                }
                else
                {
                    hitPoint = tempPoint;
                    if (Vector2.Dot(hitPoint - line.Start, line.End - line.Start) < 0 || (hitPoint - line.Start).LengthSquared() > (line.End - line.Start).LengthSquared())
                    {
                        return false;
                    }
                    hitPoint += center;
                    return true;
                }
            }
        }

        public static bool TestSegmentSegment(LineSegment line1, LineSegment line2, out Vector2 hitPoint, out bool parallel)
        {
            hitPoint = Vector2.Zero;

            Vector2 dir1 = line1.End - line1.Start;
            Vector2 perpDir1 = new Vector2(-dir1.Y, dir1.X);

            float a1 = perpDir1.X;
            float b1 = perpDir1.Y;
            float d1 = perpDir1.X * line1.Start.X + perpDir1.Y * line1.Start.Y;

            Vector2 dir2 = line2.End - line2.Start;
            Vector2 perpDir2 = new Vector2(-dir2.Y, dir2.X);

            float a2 = perpDir2.X;
            float b2 = perpDir2.Y;
            float d2 = perpDir2.X * line2.Start.X + perpDir2.Y * line2.Start.Y;

            float denom = (a1 * b2 - a2 * b1);
            if (Math.Abs(denom) < float.Epsilon)
            {
                parallel = true;
                return false;
            }
            else
            {
                hitPoint = new Vector2((b2 * d1 - b1 * d2) / denom, (a1 * d2 - a2 * d1) / denom);
                parallel = false;
                //Test bounds of segment
                //First test segment 1
                //Make sure the hit point lies between the two end points
                //First, the start point
                Vector2 startToHit = hitPoint - line1.Start;
                if (Vector2.Dot(startToHit, dir1) < 0)
                {
                    return false;
                }
                //Then the end point
                if (startToHit.LengthSquared() > dir1.LengthSquared())
                {
                    return false;
                }
                //Do the same for the second line segment
                startToHit = hitPoint - line2.Start;
                if (Vector2.Dot(startToHit, dir2) < 0)
                {
                    return false;
                }
                if (startToHit.LengthSquared() > dir2.LengthSquared())
                {
                    return false;
                }
                //If these conditions are met, the segments intersect
                return true;
            }
        }

        public static Vector2 ClosestPointCircleSegment(LineSegment seg, Vector2 center, float radius)
        {
            Vector2 startToCenter = center - seg.Start;
            Vector2 segDir = seg.End - seg.Start;
            Vector2 proj = GameMath.Project(startToCenter, segDir);

            if (Vector2.Dot(proj, segDir) <= 0)
            {
                return seg.Start;
            }
            else if (proj.LengthSquared() >= segDir.LengthSquared())
            {
                return seg.End;
            }
            else
            {
                return seg.Start + proj;
            }
        }

        //This function is responsible for taking a capsule which represents the path of a sphere over time,
        //and intersecting it against a line segment. Returns the point of collision as well as the new position of the sphere
        public static bool CollideCapsuleLineSegment(Vector2 capStart, Vector2 capEnd, float radius, LineSegment seg, out Vector2 hitPoint, out Vector2 newCirclePos)
        {
            //Intersect segment against the two segments of the capsule
            //Intersect against the two circles (the caps of the capsule)
            //Check the distances and take the closest point
            Vector2 closestPoint = Vector2.Zero;
            Vector2 tempPoint = Vector2.Zero;
            bool hit = false;
            bool collidedAtAll = false;
            bool parallel = false;

            Vector2 segDir = seg.End - seg.Start;
            Vector2 segNormal = Vector2.Normalize(new Vector2(-segDir.Y, segDir.X));
            segNormal *= radius;
            LineSegment topSeg = new LineSegment(seg.Start + segNormal, seg.End + segNormal);
            LineSegment bottomSeg = new LineSegment(seg.Start - segNormal, seg.End - segNormal);
            LineSegment capsuleSeg = new LineSegment(capStart, capEnd);

            //Test the top and bottom edges of the capsule
            hit = TestSegmentSegment(capsuleSeg, topSeg, out tempPoint, out parallel);
            if (hit)
            {
                collidedAtAll = true;
                closestPoint = tempPoint;
            }

            hit = TestSegmentSegment(capsuleSeg, bottomSeg, out tempPoint, out parallel);
            if (hit && ((tempPoint-capStart).LengthSquared() < (closestPoint-capStart).LengthSquared() || !collidedAtAll))
            {
                collidedAtAll = true;
                closestPoint = tempPoint;
            }

            //Test the two caps
            hit = TestSegmentCircle(capsuleSeg, seg.Start, radius, out tempPoint);
            if (hit && ((tempPoint - capStart).LengthSquared() < (closestPoint - capStart).LengthSquared() || !collidedAtAll))
            {
                collidedAtAll = true;                
                closestPoint = tempPoint;
            }

            hit = TestSegmentCircle(capsuleSeg, seg.End, radius, out tempPoint);
            if (hit && ((tempPoint - capStart).LengthSquared() < (closestPoint - capStart).LengthSquared() || !collidedAtAll))
            {
                collidedAtAll = true;
                closestPoint = tempPoint;
            }

            if (!collidedAtAll)
            {
                newCirclePos = Vector2.Zero;
                hitPoint = Vector2.Zero;
                return false;
            }

            newCirclePos = closestPoint;
            hitPoint = ClosestPointCircleSegment(seg, newCirclePos, radius);
            return true;
        }

        public static bool IntersectCapsuleLineSegment(Vector2 capStart, Vector2 capEnd, float radius, LineSegment seg, out Vector2 hitPoint, out Vector2 newCirclePos)
        {
            LineSegment capSeg = new LineSegment(capStart, capEnd);
            bool parallel;
            newCirclePos = Vector2.Zero;
            bool hit = TestLineLine(capSeg, seg, out hitPoint, out parallel);
            //If we didn't get a hit, return
            if (!hit && !parallel)
            {
                return false;
            }
            //If we're parallel, check to see if circle is embedded in segment, and move out
            if (parallel)
            {
                //Move circle out 
                return true;
            }

            //Check to see if our capsule segment is actually touching the line segment
            //This is inefficient, change it...
            float capSegLen = (capEnd - capStart).Length() + radius;
            if ((hitPoint - capStart).LengthSquared() > (capSegLen * capSegLen))
            {
                return false;
            }

            //Otherwise, we have a line intersection...test the bounds of the segment
            Vector2 segDir = seg.End - seg.Start;
            Vector2 hitDir = hitPoint - seg.Start;
            //The most common case is when the capsule's segment intersects the segment directly
            float hitDirLenSq = hitDir.LengthSquared();
            //If the capsule segment physically intersects the line segment
            if (Vector2.Dot(hitDir, segDir) >= 0 && hitDirLenSq <= segDir.LengthSquared())
            {
                //Move circle out back along trajectory
                Vector2 normal = new Vector2(-segDir.Y, segDir.X);
                normal.Normalize();
                normal *= radius;
                LineSegment newSeg = new LineSegment(seg.Start + normal, seg.End + normal);
                TestLineLine(newSeg, capSeg, out newCirclePos, out parallel);
                return true;
            }

            //The capsule must be intersecting at one of the endpoints of the line segment
            if (Vector2.Dot(hitDir, segDir) <= 0)
            {
                //It's near the line segment start
                if (hitDir.LengthSquared() < (radius * radius))
                {
                    //It's intersecting at the line segment start
                    //Move circle out and stuff
                    Vector2 normal = new Vector2(-segDir.Y, segDir.X);
                    normal.Normalize();
                    normal *= radius;
                    LineSegment newSeg = new LineSegment(seg.Start + normal, seg.End + normal);
                    TestLineLine(newSeg, capSeg, out newCirclePos, out parallel);
                    return true;
                }
            }
            else
            {
                //Change hitDir so we don't need any nasty square roots
                hitDir = hitPoint - seg.End;
                //It's near the line segment end
                if (hitDir.LengthSquared() < (radius * radius))
                {
                    //It's intersecting at the line segment end
                    //Move circle out and stuff
                    Vector2 normal = new Vector2(-segDir.Y, segDir.X);
                    normal.Normalize();
                    normal *= radius;
                    LineSegment newSeg = new LineSegment(seg.Start + normal, seg.End + normal);
                    TestLineLine(newSeg, capSeg, out newCirclePos, out parallel);
                    return true;
                }
            }

            return false;
        }
    }
}
