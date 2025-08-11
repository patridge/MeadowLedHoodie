using System.Collections.Generic;
using Meadow;

namespace ProjectLabLedHoodie;

class SnakeSegment
{
    public int SegmentLedIndex { get; set; }
    public Color Color { get; set; }
    public float Brightness { get; set; }
}

static class SnakeSegmentExtensions
{
    public static void Move(this LinkedListNode<SnakeSegment> segmentNode, int newIndexToMoveTo)
    {
        if (segmentNode == null)
        {
            // Shouldn't happen[?], but called on null node.
            return;
        }
        if (segmentNode.Value.SegmentLedIndex == newIndexToMoveTo)
        {
            // Segment is already at index.
            // Try to move next segment to this index instead.
            // If next node is null, then we're done.
            segmentNode.Next?.Move(newIndexToMoveTo);
        }

        int indexBeingVacated = segmentNode.Value.SegmentLedIndex;
        Resolver.Log.Debug($"Move: {indexBeingVacated} -> {newIndexToMoveTo}");
        segmentNode.Value.SegmentLedIndex = newIndexToMoveTo;
        segmentNode.Next?.Move(indexBeingVacated);
    }
}
