using UnityEngine;

namespace UnityEditor.Tilemaps
{
    static internal partial class AssetCreation
    {
        internal enum ETilesMenuItemOrder
        {
            AnimatedTile = 2,
            RuleTile = 100,
            IsometricRuleTile,
            HexagonalRuleTile,
            RuleOverrideTile,
            AdvanceRuleOverrideTile,
            CustomRuleTile,
            RandomTile = 200,
            WeightedRandomTile,
            PipelineTile,
            TerrainTile,
        }
        internal enum EBrushMenuItemOrder
        {
            RandomBrush = 3,
            PrefabBrush,
            PrefabRandomBrush
        }
        /*[MenuItem("Assets/Create/2D/Brushes/Prefab Brush", priority = (int) EBrushMenuItemOrder.PrefabBrush)]
        void CreatePrefabBrush()
        {
            //ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<PrefabBrush>(), "New Prefab Brush.asset");
        }

        /*[MenuItem("Assets/Create/2D/Brushes/Prefab Random Brush",
            priority = (int) EBrushMenuItemOrder.PrefabRandomBrush)]
        void CreatePrefabRandomBrush()
        {
            //ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<PrefabRandomBrush>(),
                //"New Prefab Random Brush.asset");
        }

        [MenuItem("Assets/Create/2D/Brushes/Random Brush", priority = (int) EBrushMenuItemOrder.RandomBrush)]
        void CreateRandomBrush()
        {
            //ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<RandomBrush>(), "New Random Brush.asset");
        }ù*/
    }
}