using System.Collections.Generic;
using TinyHelper;
using InstanceIDs;
using SideLoader;
using UnityEngine;
using System.Linq;

namespace MartialArtist
{
    class EffectInitializer
    {

        public static ImbueEffectPreset MakeHonedBladeInfusion()
        {
            ImbueEffectPreset effectPreset = TinyEffectManager.MakeImbuePreset(
                imbueID: IDs.honedBladeImbueID,
                name: "Honed Blade",
                description: "Weapon deals some Physical damage.",
                //iconFileName: Crusader.ModFolderName + @"\SideLoader\Texture2D\impendingDoomImbueIcon.png",
                visualEffectID: IDs.windImbueID
            );

            Transform effectTransform;

            effectTransform = TinyGameObjectManager.MakeFreshObject("Effects", true, true, effectPreset.transform).transform;
            TinyEffectManager.MakeWeaponDamage(effectTransform, 0, 0.20f, DamageType.Types.Physical, 0f);

            var fx = effectPreset.ImbueFX;

            fx = Object.Instantiate(fx);
            fx.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(fx);
            effectPreset.ImbueFX = fx;

            GameObject.Destroy(fx.Find("BlessParticlesSparkles").gameObject);

            return effectPreset;
        }
    }
}
