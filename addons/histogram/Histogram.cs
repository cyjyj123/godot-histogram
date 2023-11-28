using Godot;
using System;
using System.Collections.Generic;

public enum ColorType
{
    FixedColor,
    RandomColor,
    SpecifiedColor
}
public struct DataOne
{
    public string label;
    public float data;
    public DataOne(string label, float data)
    {
        this.label = label;
        this.data = data;
    }
}
public partial class Histogram : Node3D
{
    [Export(PropertyHint.MultilineText)]
    public string csvString;
    public List<DataOne> data;
    [Export]
    public float squareLength = 1;
    [Export]
    public float heightScale = 1;
    [Export]
    public float distanceX=1;
    [Export]
    public float distanceLabel = 0.5f;
    private int count = 0;
    [Export]
    public Camera3D mainCamera;
    [Export]
    public Font font;
    [Export]
    public ColorType colorType = ColorType.FixedColor;
    [Export]
    public Color color=new Color(0xca90e900);
    [Export]
    public Godot.Collections.Array<Color> colors;
    private Node3D nodes_node;
    public override void _Ready()
    {
        data=new List<DataOne> ();
        var csvLines = csvString.Split("\n");
        foreach ( var line in csvLines )
        {
            var eles=line.Split(",");
            data.Add(new DataOne(eles[0], (float)Convert.ToDouble(eles[1])));
        }
    }
    public void Play()
    {
        // Show
        if (nodes_node != null)
        {
            RemoveChild(nodes_node);
        }
        count = 0;
        nodes_node = new Node3D();
        this.AddChild(nodes_node);
        UpdateAData();
    }

    private void UpdateAData()
    {
        var v = data[count];
        var v_node = new Node3D();
        var v_mesh = new MeshInstance3D();
        var v_shape = new BoxMesh();
        var v_mat = new StandardMaterial3D();
        if (colorType == ColorType.FixedColor)
        {
            v_mat.AlbedoColor = color;
        } else if (colorType == ColorType.RandomColor)
        {
            var r = new Random();
            v_mat.AlbedoColor = new Color(r.Next(256) / 255.0f, r.Next(256) / 255.0f, r.Next(256) / 255.0f);
        } else if (colorType == ColorType.SpecifiedColor) {
            v_mat.AlbedoColor = colors[count%colors.Count];
        } else
        {

        }
        v_shape.Size = new Vector3(squareLength, 0, squareLength);
        v_mesh.Mesh = v_shape;
        v_mesh.SetSurfaceOverrideMaterial(0,v_mat);
        
        v_node.AddChild(v_mesh);
        v_mesh.Position = new Vector3((distanceX + squareLength) * count, 0, 0);
        nodes_node.AddChild(v_node);

        var tween = GetTree().CreateTween();
        tween.TweenProperty(v_shape, "size:y", v.data * heightScale, 1);
        tween.Parallel().TweenProperty(v_node, "position:y", v.data * heightScale / 2, 1);

        var labelPosV3 = new Vector3((distanceX + squareLength) * count, v.data * heightScale + distanceLabel, 0);
        var labelPos = mainCamera.UnprojectPosition(labelPosV3);
        var showLabel = new Label();
        if (font != null)
        {
            showLabel.AddThemeFontOverride("font", font);
        }
        showLabel.Text = v.label;
        var labelPosYBase = mainCamera.UnprojectPosition(new Vector3((distanceX + squareLength) * count, 0, 0));
        showLabel.Position = new Vector2(labelPos.X, labelPosYBase.Y); ;
        nodes_node.AddChild(showLabel);

        tween.Parallel().TweenProperty(showLabel, "position:y", labelPos.Y, 1);

        if (count != data.Count - 1)
        {
            count++;
            tween.Finished += UpdateAData;
        }
    }
}
