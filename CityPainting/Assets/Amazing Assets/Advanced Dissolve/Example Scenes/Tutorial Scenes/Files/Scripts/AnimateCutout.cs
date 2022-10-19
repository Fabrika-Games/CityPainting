using UnityEngine;

using AmazingAssets.AdvancedDissolve;


namespace AmazingAssets.AdvancedDissolve.ExampleScripts
{
    public class AnimateCutout : MonoBehaviour
    {
        Material material;

        float offset;
        float speed;        

        private void Start()
        {
            GetComponent<Renderer>();

            material = GetComponent<Renderer>().material;

            offset = Random.value;
            speed = Random.Range(0.1f, 0.2f);
        }

        // Update is called once per frame
        void Update()
        {
            float clip = Mathf.PingPong(offset + Time.time * speed, 1);

            AmazingAssets.AdvancedDissolve.AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, clip);
        }
    }
}