﻿//////////////////////////////////////////////////////
// MK Toon Built-in Standard PBS Editor        		//
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2021 All rights reserved.            //
//////////////////////////////////////////////////////

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using UnityEditor.Utils;
using UnityEditorInternal;
using EditorHelper = MK.Toon.Editor.EditorHelper;

namespace MK.Toon.Editor.Legacy
{
    internal class StandardPBSEditor : MK.Toon.Editor.PhysicallyBasedEditorBase 
    {
        protected override void DrawReceiveShadows(MaterialEditor materialEditor) {}
    }
}
#endif