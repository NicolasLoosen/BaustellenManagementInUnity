using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides methods to draw shapes around objects with line renderer and remove them as well
/// </summary>
public static class ShapeDrawer
{
    /// <summary>
    /// This method draws a circle around a game object or removes it if it is already drawn
    /// </summary>
    public static void DrawCircle(this GameObject container, float radius, float lineWidth, Color color)
    {
        //checks if linerenderer is already added to the object and adds it if not
        if (container.GetComponent<LineRenderer>() == null)
        {
            container.AddComponent<LineRenderer>();
            container.GetComponent<LineRenderer>().positionCount = 0;
        }
        circle(container, radius, lineWidth, color);
    }

    /// <summary>
    /// This method draws a rectangle around a game object or removes it if it is already drawn
    /// </summary>
    public static void DrawRectangle(this GameObject container, float aLine, float bLine, float lineWidth, Color color)
    {
        //line renderer gets added in case it isnt already
        if (container.GetComponent<LineRenderer>() == null)
        {
            container.AddComponent<LineRenderer>();
            container.GetComponent<LineRenderer>().positionCount = 0;
        }
        rectangle(container, aLine, bLine, lineWidth, color);
    }

    /// <summary>
    /// This method draws a line under a game object or removes it if it is already drawn
    /// </summary>
    public static void DrawLine(this GameObject container, float length, float lineWidth, Color color)
    {
        //line renderer gets added in case it isnt already
        if (container.GetComponent<LineRenderer>() == null)
        {
            container.AddComponent<LineRenderer>();
            container.GetComponent<LineRenderer>().positionCount = 0;
        }
        line(container, length, lineWidth, color);
    }


    //draw methods follow
    private static void circle(GameObject container, float radius, float lineWidth, Color color)
    {
        var segments = 360;
        var line = container.GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(color, color);
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 0;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        //points for the lines of the circle get defined
        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
    }

    private static void rectangle(GameObject container, float aLine, float bLine, float lineWidth, Color color)
    {
        var line = container.GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(color, color);
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 0;
        line.positionCount = 5;

        var pointCount = 5;
        var points = new Vector3[pointCount];

        //points that connect the lines get defined
        points[0] = new Vector3(aLine / 2, 0, bLine / 2);
        points[1] = new Vector3(-(aLine / 2), 0, bLine / 2);
        points[2] = new Vector3(-(aLine / 2), 0, -(bLine / 2));
        points[3] = new Vector3(aLine / 2, 0, -(bLine / 2));
        points[4] = new Vector3(aLine / 2, 0, bLine / 2);

        line.SetPositions(points);
    }

    private static void line(GameObject container, float length, float lineWidth, Color color)
    {
        var line = container.GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(color, color);
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 0;
        line.positionCount = 2;

        var pointCount = 2;
        var points = new Vector3[pointCount];

        //start and endpoint of the line gets defined
        points[0] = new Vector3(0, 0, length / 2);
        points[1] = new Vector3(0, 0, -(length / 2));

        line.SetPositions(points);
    }

    public static void deleteLines(this GameObject container)
    {
        var line = container.GetComponent<LineRenderer>();
        line.positionCount = 0;
    }
}
