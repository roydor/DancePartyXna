using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanceParty.Actors
{
    public enum DancerType
    {
        MaleSuit,
        MaleSalsa,
        FemaleBlueStripesDress,
        FemaleBlackDots,
    }

    public enum DancerGender
    {
        Male,
        Female,
    }

    public struct DancerTypeData
    {
        public DancerGender Gender;
        public DancerType DancerType;
        public string ModelAssetPath;
        public string TextureAssetPath;
    }

    public static class DancerTypes
    {
        public static DancerTypeData[] Dancers = new DancerTypeData[] 
        {
            new DancerTypeData()
            { 
                DancerType = DancerType.MaleSuit, 
                ModelAssetPath = "Models\\male_low", 
                TextureAssetPath="Textures\\male0_0",
                Gender = DancerGender.Male,
            },
            new DancerTypeData()
            { 
                DancerType = DancerType.MaleSalsa, 
                ModelAssetPath = "Models\\male_low", 
                TextureAssetPath="Textures\\male0_1",
                Gender = DancerGender.Male,
            },
            new DancerTypeData() 
            {
                DancerType = DancerType.FemaleBlueStripesDress,
                ModelAssetPath = "Models\\female2",
                TextureAssetPath="Textures\\female2_0",
                Gender = DancerGender.Female,
            },
            new DancerTypeData()
            {
                DancerType = DancerType.FemaleBlackDots,
                ModelAssetPath = "Models\\female1",
                TextureAssetPath="Textures\\female1_0",
                Gender = DancerGender.Female,
            },
        };
    }
}
