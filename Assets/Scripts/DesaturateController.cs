//    Copyright (C) 2020 Ned Makes Games

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DesaturateController : MonoBehaviour {

    [SerializeField] private ForwardRendererData rendererData = null;
    [SerializeField] private string featureName = null;
    [SerializeField] private float transitionPeriod = 1;

    private bool transitioning;
    private float startTime;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartTransition();
        }
        if(transitioning) {
            if(Time.timeSinceLevelLoad >= startTime + transitionPeriod) {
                EndTransition();
            } else {
                UpdateTransition();
            }
        }
    }

    private void OnDestroy() {
        ResetTransition();
    }

    private bool TryGetFeature(out ScriptableRendererFeature feature) {
        feature = rendererData.rendererFeatures.Where((f) => f.name == featureName).FirstOrDefault();
        return feature != null;
    }

    private void StartTransition() {
        startTime = Time.timeSinceLevelLoad;
        transitioning = true;
    }

    private void UpdateTransition() {
        if(TryGetFeature(out var feature)) {
            float saturation = Mathf.Clamp01((Time.timeSinceLevelLoad - startTime) / transitionPeriod);

            var blitFeature = feature as BlitMaterialFeature;
            var material = blitFeature.Material;
            material.SetFloat("_Saturation", saturation);
        }
    }

    private void EndTransition() {
        if(TryGetFeature(out var feature)) {
            feature.SetActive(false);
            rendererData.SetDirty();

            transitioning = false;
        }
    }

    private void ResetTransition() {
        if(TryGetFeature(out var feature)) {
            feature.SetActive(true);
            rendererData.SetDirty();

            var blitFeature = feature as BlitMaterialFeature;
            var material = blitFeature.Material;
            material.SetFloat("_Saturation", 0);
            
            transitioning = false;
        }
    }
}
