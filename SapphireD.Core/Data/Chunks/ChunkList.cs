using SapphireD.Core.Data.Chunks.AppChunks;
using SapphireD.Core.Data.Chunks.BankChunks;
using SapphireD.Core.Data.Chunks.BankChunks.Fonts;
using SapphireD.Core.Data.Chunks.BankChunks.Images;
using SapphireD.Core.Data.Chunks.BankChunks.Sounds;
using SapphireD.Core.Data.Chunks.FrameChunks;
using SapphireD.Core.Data.Chunks.MFAChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks;
using SapphireD.Core.Data.Chunks.ObjectChunks.ObjectCommon;

namespace SapphireD.Core.Data.Chunks
{
    public class ChunkList
    {
        public static Dictionary<short, Type> ChunkJumpTable = new Dictionary<short, Type>()
        {
            { 0x0016, typeof(MFACounterFlags)     },
            { 0x0021, typeof(FrameRect)           },
            //0x0022         FrameDemoPath
            { 0x0023, typeof(FrameSeed)           },
            //0x0025         FrameLayerEffects
            //0x0026
            { 0x0027, typeof(FrameMoveTimer)      },
            //0x0028         FrameEffects
            //0x002A
            //0x002B
            //0x002C
            //0x002D         ObjectEffects
            //0x002E
            //0x002F
            //0x0030
            //0x0031         FrameInclude
            { 0x0039, typeof(MFAAltFlags)         },
            { 0x003A, typeof(MFAAltValueIndex)    },
            { 0x003B, typeof(MFAAltStringIndex)   },
            { 0x003C, typeof(MFAAltFlagIndex)     },
            //0x1122         Preview
            //0x2222         MiniHeader
            { 0x2223, typeof(AppHeader)           },
            { 0x2224, typeof(AppName)             },
            { 0x2225, typeof(Author)              },
            { 0x2226, typeof(MenuBar)             },
            { 0x2227, typeof(ExtensionsPath)      },
            //0x2228         Extensions
            { 0x2229, typeof(FrameItems)          },
            //0x222A         GlobalEvents
            { 0x222B, typeof(FrameHandles)        },
            { 0x222C, typeof(ExtensionData)       },
            //0x222D         ExtraExtensions
            { 0x222E, typeof(EditorFilename)      },
            { 0x222F, typeof(TargetFilename)      },
            { 0x2230, typeof(HelpFile)            },
            { 0x2231, typeof(TransitionFile)      },
            { 0x2232, typeof(GlobalValues)        },
            { 0x2233, typeof(GlobalStrings)       },
            { 0x2234, typeof(Extensions)          },
            { 0x2235, typeof(AppIcon)             },
            //0x2236         IsDemo
            { 0x2237, typeof(SerialNumber)        },
            { 0x2238, typeof(BinaryFiles)         },
            //0x2239         MenuImages
            { 0x223A, typeof(About)               },
            { 0x223B, typeof(Copyright)           },
            { 0x223C, typeof(GlobalValueNames)    },
            { 0x223D, typeof(GlobalStringNames)   },
            //0x223E         MovementExtensions
            { 0x223F, typeof(FrameItems)          },
            { 0x2240, typeof(ExeOnly)             },
            //0x2241
            { 0x2242, typeof(Protection)          },
            //0x2243         Shaders
            //0x2244
            { 0x2245, typeof(ExtendedHeader)      },
            { 0x2246, typeof(AppCodePage)         },
            //0x2247         FrameOffset
            //0x2248         AdMobID
            //0x2249
            //0x224A
            //0x224B         AndroidMenu
            //0x224C
            //0x224D
            //0x224E
            //0x224F
            //0x2250
            //0x2251
            //0x2252
            //0x2253         ObjectHeaders
            //0x2254         ObjectNames
            //0x2255         ObjectShaders
            //0x2256         ObjectProperties
            //0x2257
            //0x2258         TrueTypeFontInfo
            //0x2259         TrueTypeFonts
            //0x225A         Shaders
            //0x225B
            //0x225C
            //0x225D
            //0x225E
            //0x225F
            //0x2260
            { 0x3333, typeof(Frame)               },
            { 0x3334, typeof(FrameHeader)         },
            { 0x3335, typeof(FrameName)           },
            { 0x3336, typeof(FramePassword)       },
            { 0x3337, typeof(FramePalette)        },
            { 0x3338, typeof(FrameInstances)      },
            //0x3339         FrameFadeInStuff
            //0x333A         FrameFadeOutStuff
            { 0x333B, typeof(FrameTransitionIn)   },
            { 0x333C, typeof(FrameTransitionOut)  },
            { 0x333D, typeof(FrameEvents)         },
            //0x333E         FramePlayHeader
            //0x333F         FrameExtraItems
            //0x3340         FrameExtraInstances
            { 0x3341, typeof(FrameLayers)         },
            { 0x3342, typeof(FrameRect)           },
            //0x3343         FrameDemoPath
            { 0x3344, typeof(FrameSeed)           },
            { 0x3345, typeof(FrameLayerEffects)   },
            //0x3346         FrameBluRayOptions
            { 0x3347, typeof(FrameMoveTimer)      },
            //0x3348         FrameMosiacTable
            { 0x3349, typeof(FrameEffects)        },
            //0x334A         FrameiPhoneOptions
            //0x334B
            //0x334C
            { 0x4444, typeof(ObjectInfoHeader)    },
            { 0x4445, typeof(ObjectInfoName)      },
            { 0x4446, typeof(ObjectCommon)        },
            //0x4447
            //0x4448         ObjectEffects
            { 0x4449, typeof(ObjectAnimations)    },
            { 0x444A, typeof(AnimationOffsets)    },
            //0x4500         ObjectShapes
            { 0x5555, typeof(ImageOffsets)        },
            { 0x5556, typeof(FontOffsets)         },
            { 0x5557, typeof(SoundOffsets)        },
            { 0x5558, typeof(MusicOffsets)        },
            //0x6665         BankOffsets
            { 0x6666, typeof(ImageBank)           },
            { 0x6667, typeof(FontBank)            },
            { 0x6668, typeof(SoundBank)           },
            //0x6669         MusicBank
            //0x7EEE         Fusion3Seed
            { 0x7F7F, typeof(Last)                }
        };
    }
}
