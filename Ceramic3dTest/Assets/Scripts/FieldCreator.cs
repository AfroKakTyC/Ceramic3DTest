using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldCreator : MonoBehaviour
{
    [SerializeField]
    GameObject FieldPrefab;
    [SerializeField]
    InputField HeightInputField;
    [SerializeField]
    InputField WidthInputField;
    [SerializeField]
    InputField AngleInputField;
    [SerializeField]
    InputField SeamInputField;
    [SerializeField]
    InputField OffsetInputField;

    [SerializeField]
    TilesCreator TilesCreatorScript;

    public float FieldWidthInM { private set; get; }
    public float FieldHeightInM { private set; get; }

    public GameObject field { private set; get; }
    public Transform TilesHolder { private set; get; }
    public Field FieldScript { private set; get; }

    private GameObject fieldHolder;

    public void CreateField()
	{
        if (fieldHolder)
		{
            Destroy(fieldHolder);
		}
		TilesCreatorScript.SetSeamSize(float.Parse(SeamInputField.text) * 0.001f);
		TilesCreatorScript.SetOffset(float.Parse(OffsetInputField.text) * 0.001f);

		FieldWidthInM = int.Parse(WidthInputField.text) * 0.01f;
        FieldHeightInM = int.Parse(HeightInputField.text) * 0.01f;
        int rotationAngle = int.Parse(AngleInputField.text);

        field = Instantiate(FieldPrefab, Vector3.zero, Quaternion.identity);
        field.GetComponent<MeshFilter>().mesh.vertices = new Vector3[4]
        {
            new Vector3(0, 0),
            new Vector3(FieldWidthInM, 0),
            new Vector3(0, FieldHeightInM),
            new Vector3(FieldWidthInM, FieldHeightInM)
        };
        fieldHolder = new GameObject("FieldHolder");
        fieldHolder.transform.position = new Vector3(FieldWidthInM / 2, FieldHeightInM / 2, 0f);
        field.transform.SetParent(fieldHolder.transform);
        fieldHolder.transform.position += new Vector3(0f, 0f, 0.001f);
        fieldHolder.transform.Rotate(new Vector3(0f, 0f, rotationAngle));

        GameObject tilesHolder = new GameObject("TilesHolder");
        TilesHolder = tilesHolder.transform;
        tilesHolder.transform.SetParent(fieldHolder.transform);

        field.AddComponent<Field>().CalculateFieldVerticesWorldPosition();
        FieldScript = field.GetComponent<Field>();
        TilesCreatorScript.CreateTiles();

        fieldHolder.transform.Rotate(new Vector3(0f, 0f, -rotationAngle));
    }
}
