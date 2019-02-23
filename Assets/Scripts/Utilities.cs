using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

public sealed class Utilities
{
    internal static Vector3 ConvertDirection(eDirections dir)
    {
        switch (dir)
        {
            case eDirections.BACKWARD:
                return new Vector3(0f, 0f, -1f);
            case eDirections.FORWARD:
                return new Vector3(0f, 0f, 1f);
            case eDirections.UP:
                return new Vector3(0f, 1f, 0f);
            case eDirections.DOWN:
                return new Vector3(0f, -1f, 0f);
            case eDirections.RIGHT:
                return new Vector3(1f, 0f, 0f);
            case eDirections.LEFT:
                return new Vector3(-1f, 0f, 0f);
        }

        return Vector3.forward;
    }

    internal static eDirections ConvertDirection(Vector3 dir)
    {
        if (dir == Vector3.forward)
        {
            return eDirections.FORWARD;
        }
        else if (dir == Vector3.back)
        {
            return eDirections.BACKWARD;
        }
        else if (dir == Vector3.up)
        {
            return eDirections.UP;
        }
        else if (dir == Vector3.down)
        {
            return eDirections.DOWN;
        }
        else if (dir == Vector3.right)
        {
            return eDirections.RIGHT;
        }
        else if (dir == Vector3.left)
        {
            return eDirections.LEFT;
        }

        return eDirections.FORWARD;
    }

    internal static eDirections GetRandomDirection()
    {
        eDirections[] dirs = GetDirectionsArray();

        int rnd = URandom.Range(0, dirs.Length);

        return dirs[rnd];
    }

    internal static eDirections GetRandomDirection(eDirections excludeDir)
    {
        List<eDirections> dirs = GetDirectionsList();
        dirs.Remove(excludeDir);
        dirs.Remove(GetOppositeDirection(excludeDir));

        int rnd = URandom.Range(0, dirs.Count);

        return dirs[rnd];
    }

    internal static eDirections GetOppositeDirection(eDirections excludeDir)
    {
        switch (excludeDir)
        {
            case eDirections.FORWARD:
                return eDirections.BACKWARD;
            case eDirections.BACKWARD:
                return eDirections.FORWARD;
            case eDirections.UP:
                return eDirections.DOWN;
            case eDirections.DOWN:
                return eDirections.UP;
            case eDirections.RIGHT:
                return eDirections.LEFT;
            case eDirections.LEFT:
                return eDirections.RIGHT;
        }

        return eDirections.BACKWARD;
    }

    internal static eDirections[] GetDirectionsArray()
    {
        return (eDirections[])Enum.GetValues(typeof(eDirections));
    }

    internal static List<eDirections> GetDirectionsList()
    {
        return GetDirectionsArray().ToList();
    }

    internal static eDirections GetNextCellDirection(int[] cellPos, int[] neighbourPos)
    {
        if (cellPos[0] == neighbourPos[0] && cellPos[1] == neighbourPos[1] && cellPos[2] + 1 == neighbourPos[2])
        {
            return eDirections.FORWARD;
        }
        else if (cellPos[0] == neighbourPos[0] && cellPos[1] == neighbourPos[1] && cellPos[2] - 1 == neighbourPos[2])
        {
            return eDirections.BACKWARD;
        }
        else if (cellPos[0] == neighbourPos[0] && cellPos[1] + 1 == neighbourPos[1] && cellPos[2] == neighbourPos[2])
        {
            return eDirections.UP;
        }
        else if (cellPos[0] == neighbourPos[0] && cellPos[1] - 1 == neighbourPos[1] && cellPos[2] == neighbourPos[2])
        {
            return eDirections.DOWN;
        }
        else if (cellPos[0] + 1 == neighbourPos[0] && cellPos[1] == neighbourPos[1] && cellPos[2] == neighbourPos[2])
        {
            return eDirections.RIGHT;
        }
        else if (cellPos[0] - 1 == neighbourPos[0] && cellPos[1] == neighbourPos[1] && cellPos[2] == neighbourPos[2])
        {
            return eDirections.LEFT;
        }

        return eDirections.FORWARD;
    }
}
