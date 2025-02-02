﻿using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using OpenCBS.ArchitectureV2.Interface;
using OpenCBS.ArchitectureV2.Interface.Presenter;
using OpenCBS.ArchitectureV2.Interface.Service;
using OpenCBS.ArchitectureV2.Interface.View;
using StructureMap;

namespace OpenCBS.ArchitectureV2
{
    public class AppContext : ApplicationContext
    {
        private readonly IContainer _container;

        public AppContext(IContainer container)
        {
            _container = container;
            var databaseService = _container.GetInstance<IDatabaseService>();
            if (databaseService.IsServerConnectionOk())
            {
                foreach (var startupProcess in _container.GetAllInstances<IStartupProcess>())
                {
                    startupProcess.Run();
                }
                MainForm = GetLoginForm();
            }
            else
            {
                MainForm = GetSqlServerConnectionForm();
            }
        }
        
        private Form GetLoginForm()
        {
            var presenter = _container.GetInstance<ILoginPresenter>();
            presenter.Run();
            return (Form)presenter.View;
        }

        private Form GetLoginVerificationForm()
        {
            var presenter = _container.GetInstance<ILoginVerificationPresenter>();
            presenter.Run();
            return (Form)presenter.View;
        }

        private Form GetChangePasswordForm()
        {
            var presenter = _container.GetInstance<IChangePasswordPresenter>();
            presenter.Run();
            return (Form)presenter.View;
        }

        private Form GetMainForm()
        {
            var settingsService = _container.GetInstance<ISettingsService>();
            settingsService.Init();

            var culture = (CultureInfo) CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = settingsService.GetShortDateFormat();
            culture.NumberFormat = new NumberFormatInfo
            {
                NumberGroupSeparator = settingsService.GetNumberGroupSeparator(),
                NumberDecimalSeparator = settingsService.GetNumberDecimalSeparator(),
            };
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var mainView = _container.GetInstance<IMainView>();
            mainView.Run();
            return (Form) mainView;
        }

        private Form GetUpgradeForm()
        {
            var presenter = _container.GetInstance<IUpgradePresenter>();
            presenter.Run();
            return (Form)presenter.View;
        }

        private Form GetSqlServerConnectionForm()
        {
            var presenter = _container.GetInstance<ISqlServerConnectionPresenter>();
            presenter.Run();
            return (Form)presenter.View;
        }

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            if (sender is ILoginView)
            {
                var authService = _container.GetInstance<IAuthService>();
                if (authService.LoggedIn)
                {
                    var databaseService = _container.GetInstance<IDatabaseService>();
                    if (databaseService.IsVersionOk())
                    {
                        if (authService.AllowAccess)
                            MainForm = GetMainForm();
                        else if (authService.LoginReset)
                            MainForm = GetChangePasswordForm();
                        else if (authService.RequiresVerification)
                            MainForm = GetLoginVerificationForm();
                        else
                            base.OnMainFormClosed(sender, e);
                    }
                    else
                    {
                        MainForm = GetUpgradeForm();
                    }
                }
                else
                {
                    base.OnMainFormClosed(sender, e);
                }
            }
            else if (sender is ILoginVerificationView)
            {
                var authService = _container.GetInstance<IAuthService>();
                if (authService.LoggedIn)
                {
                    var databaseService = _container.GetInstance<IDatabaseService>();
                    if (databaseService.IsVersionOk())
                    {
                        if (authService.AllowAccess)
                            MainForm = GetMainForm();
                        else
                            Restart.LaunchRestarter();
                        //base.OnMainFormClosed(sender, e);
                        //MainForm = GetLoginForm();
                    }
                    else
                    {
                        MainForm = GetUpgradeForm();
                    }
                }
                else
                {
                    base.OnMainFormClosed(sender, e);
                }
            }
            else if (sender is IPasswordView)
            {
                var authService = _container.GetInstance<IAuthService>();
                if (authService.LoggedIn)
                {
                    var databaseService = _container.GetInstance<IDatabaseService>();
                    if (databaseService.IsVersionOk())
                    {
                        if (authService.AllowAccess)
                            MainForm = GetMainForm();
                        else
                            Restart.LaunchRestarter();
                    }
                    else
                    {
                        MainForm = GetUpgradeForm();
                    }
                }
                else
                {
                    base.OnMainFormClosed(sender, e);
                }
            }
            else if (sender is IUpgradeView)
            {
                var view = (IUpgradeView)sender;
                if (view.Upgraded)
                {
                    MainForm = GetMainForm();
                }
                else
                {
                    base.OnMainFormClosed(sender, e);
                }
            }
            else if (sender is IMainView)
            {
                base.OnMainFormClosed(sender, e);
            }
        }
    }
}
