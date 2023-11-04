using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableItemEventSubscriber : MonoBehaviour
{

    private void Start() {
        GhostObjects ghostObject = GetComponent<GhostObjects>();
        ghostObject.OnGhostInteraction += GhostObject_OnGhostInteraction;
    }

    private void GhostObject_OnGhostInteraction(object sender, EventArgs e) {
        
    }
}
