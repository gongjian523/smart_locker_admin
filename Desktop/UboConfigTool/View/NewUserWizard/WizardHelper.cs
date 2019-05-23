using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UboConfigTool.Infrastructure.UserGuidBase;

namespace UboConfigTool.View
{
    public class WizardHelper : DependencyObject
    {
        public static readonly DependencyProperty ProcessCompletionProperty = DependencyProperty.RegisterAttached(
        "ProcessCompletion", typeof( double ), typeof( WizardHelper ), new PropertyMetadata( 0.0, OnProcessCompletionChanged ) );

        private static readonly DependencyPropertyKey ProcessStagePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
        "ProcessStage", typeof( WizardProcedureType ), typeof( WizardHelper ), new PropertyMetadata( WizardProcedureType.AddUBOGroup ) );

        public static readonly DependencyProperty ProcessStageProperty = ProcessStagePropertyKey.DependencyProperty;

        private static void OnProcessCompletionChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            double progress = (double)e.NewValue;
            ProgressBar bar = d as ProgressBar;
            if( progress >= 0 && progress < 20 )
                bar.SetValue( ProcessStagePropertyKey, WizardProcedureType.AddUBOGroup );
            if( progress >= 20 && progress < 40 )
                bar.SetValue( ProcessStagePropertyKey, WizardProcedureType.AddUBO );
            if( progress >= 40 && progress < 60 )
                bar.SetValue( ProcessStagePropertyKey, WizardProcedureType.AddRules );
            if( progress >= 60 && progress < 80 )
                bar.SetValue( ProcessStagePropertyKey, WizardProcedureType.AddUSBs );
            if( progress >= 80 && progress <= 100 )
                bar.SetValue( ProcessStagePropertyKey, WizardProcedureType.Finish );
        }

        public static void SetProcessCompletion( ProgressBar bar, double progress )
        {
            bar.SetValue( ProcessCompletionProperty, progress );
        }

        public static void SetProcessStage( ProgressBar bar, WizardProcedureType stage )
        {
            bar.SetValue( ProcessStagePropertyKey, stage );
        }
    }
}
