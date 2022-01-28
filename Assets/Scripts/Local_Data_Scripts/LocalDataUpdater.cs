using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;
using System;

public static class LocalDataUpdater
{
    public static void Update(
        UserDataManager userDataManager, AreaDataManager areaDataManager, GssDbHub gssDbHub, Action userFeedback = null, Action areaFeedback = null)
    {
        userDataManager.UpdateAllUserNamesToGss(gssDbHub, userFeedback);
        areaDataManager.UpdateAllDatasToGss(gssDbHub, areaFeedback);
    }
}
