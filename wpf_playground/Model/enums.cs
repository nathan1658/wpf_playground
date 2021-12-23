using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{

    public enum ClickState
    {
        NA = -1,
        Correct,
        Incorrect,
        Miss
    }

    public enum HistoryType
    {
        Click,
        PQ,
        Signal
    }

    public enum GenderEnum
    {
        Male,
        Female
    }

    public enum DominantHandEnum
    {
        Left,
        Right
    }

    public enum LevelEnum
    {
        L50,
        L75,
        L100
    }


    public enum SignalModeEnum
    {
        Visual,
        Auditory,
        Tactile
    }

    public enum PQModeEnum
    {
        Visual,
        Auditory,
        Tactile
    }

    public enum SOAEnum
    {
        Soa200,
        Soa600,
        Soa1000
    }

    public enum MappingEnum
    {
      NONE,BC,TC,LC,BI
    }

}

