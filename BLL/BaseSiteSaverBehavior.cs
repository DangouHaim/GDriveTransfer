using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    class BaseSiteSaverBehavior
    {
        public static ISiteSaverBehavior Behavior { get; private set; }

        public BaseSiteSaverBehavior(ISiteSaverBehavior behavior)
        {
            Behavior = behavior;
            DAL.SiteSaver.SetBehavior(new SiteSaverBehavior());
        }

        private class SiteSaverBehavior : DAL.ISiteBehavior
        {
            public void PressAddressCombination(string address)
            {
                Behavior.PressAddressCombination(address);
            }

            public void PressEnter()
            {
                Behavior.PressEnter();
            }

            public void PressSavingCombination()
            {
                Behavior.PressSavingCombination();
            }

            public void PressTypeSelectionCombination()
            {
                Behavior.PressTypeSelectionCombination();
            }
        }
    }
}
