// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.UI;

using PolyToolkit;
using System.Collections.Generic;

/// <summary>
/// Simple example that loads and displays one asset.
/// 
/// This example requests a specific asset and displays it.
/// </summary>
public class GooglePolyModel : MonoBehaviour
{

    // ATTENTION: Before running this example, you must set your API key in Poly Toolkit settings.
    //   1. Click "Poly | Poly Toolkit Settings..."
    //      (or select PolyToolkit/Resources/PtSettings.asset in the editor).
    //   2. Click the "Runtime" tab.
    //   3. Enter your API key in the "Api key" box.
    //
    // This example does not use authentication, so there is no need to fill in a Client ID or Client Secret.

    // Number of assets imported so far.
    private int assetCount = 0;

    // Text field where we display the attributions (credits) for the assets we display.
    //public Text attributionsText;

    // Status bar text.
    // public Text statusText;

    private void Start()
    {
        // Request a list of featured assets from Poly.
        Debug.Log("Getting featured assets...");
        //statusText.text = "Requesting...";

        PolyListAssetsRequest request = PolyListAssetsRequest.Latest();
        // Limit requested models to those of medium complexity or lower.
        request.maxComplexity = PolyMaxComplexityFilter.MEDIUM;
        //request.category = PolyCategory.OBJECTS;
        request.keywords = "dj";
        request.curated = true;
        PolyApi.ListAssets(request, ListAssetsCallback);
    }

    // Callback invoked when the featured assets results are returned.
    private void ListAssetsCallback(PolyStatusOr<PolyListAssetsResult> result)
    {
        if (!result.Ok)
        {
            Debug.LogError("Failed to get featured assets. :( Reason: " + result.Status);
            // statusText.text = "ERROR: " + result.Status;
            return;
        }
        Debug.Log("Successfully got featured assets!");
        //statusText.text = "Importing...";

        // Set the import options.
        PolyImportOptions options = PolyImportOptions.Default();
        // We want to rescale the imported meshes to a specific size.
        options.rescalingMode = PolyImportOptions.RescalingMode.FIT;
        // The specific size we want assets rescaled to (fit in a 1x1x1 box):
        options.desiredSize = 1.0f;
        // We want the imported assets to be recentered such that their centroid coincides with the origin:
        options.recenter = true;

        // Now let's get the first 5 featured assets and put them on the scene.
        List<PolyAsset> assetsInUse = new List<PolyAsset>();
        //for (int i = 0; i < Mathf.Min(1, result.Value.assets.Count); i++)
        //{
        // Import this asset.
        PolyAsset poly = result.Value.assets[Random.Range(0, 10)];
        PolyApi.Import(poly, options, ImportAssetCallback);
        assetsInUse.Add(poly);
        //}

        // Show attributions for the assets we display.
        // attributionsText.text = PolyApi.GenerateAttributions(includeStatic: true, runtimeAssets: assetsInUse);
    }

    // Callback invoked when an asset has just been imported.
    private void ImportAssetCallback(PolyAsset asset, PolyStatusOr<PolyImportResult> result)
    {
        if (!result.Ok)
        {
            Debug.LogError("Failed to import asset. :( Reason: " + result.Status);
            return;
        }

        // Position each asset evenly spaced from the next.
        //assetCount++;
        //result.Value.gameObject.transform.position = new Vector3(assetCount * 1.5f, 0f, 0f);
        result.Value.gameObject.transform.parent = transform.GetChild(0);
        result.Value.gameObject.transform.localPosition = Vector3.zero;
        //AudioSource audioSource = result.Value.gameObject.transform.parent.GetComponents<AudioSource>();
        //_AudioController.Instance.SetTrack(audioSource);

        for (int i = 0; i < _AudioController.Instance.slaves.Length; i++)
        {
            _AudioController.Instance.slaves[i].volume = 1;
            //_AudioController.Instance.slaves[i].spatialBlend = 0;
        }

        // statusText.text = "Imported " + assetCount + " assets";
    }
}