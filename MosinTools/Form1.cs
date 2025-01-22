using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using MetroFramework.Components;

namespace MosinTools
{
    public partial class MosinTools : MetroFramework.Forms.MetroForm
    {
        public MosinTools()
        {
            InitializeComponent();
        }

        private void MosinTools_Load(object sender, EventArgs e)
        {

        }

        private void DownloadFileWithProgress(string url, string destinationPath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        progressBar.Value = e.ProgressPercentage;
                        lblStatus.Text = $"다운로드 중: {url} ({e.ProgressPercentage}%)";
                    };

                    client.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            MessageBox.Show($"다운로드 중 오류 발생: {e.Error.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    client.DownloadFileAsync(new Uri(url), destinationPath);
                    while (client.IsBusy) Application.DoEvents(); // 다운로드 완료까지 대기
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"다운로드 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsAnyCheckboxChecked(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is CheckBox checkBox && checkBox.Checked)
                {
                    return true; // 체크된 체크박스를 발견하면 true 반환
                }
                // 컨테이너 컨트롤(GroupBox, Panel 등)의 자식 컨트롤도 확인
                if (control.HasChildren && IsAnyCheckboxChecked(control))
                {
                    return true;
                }
            }
            return false;
        }

        private void btnDownload_Click_1(object sender, EventArgs e)
        {
            // 호출 코드
            if (!IsAnyCheckboxChecked(this))
            {
                MessageBox.Show("다운로드할 파일을 선택해주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 다운로드 폴더 경로 설정
            string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "download");

            // 폴더가 존재하지 않으면 생성
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }

            // 체크박스와 URL 매핑
            var fileLinks = new[]
            {
                new { CheckBox = chromesel, Url = "https://mc-cdn.xyz/mosintools/ChromeSetup.exe", FileName = "ChromeSetup.exe" },
                new { CheckBox = firefoxsel, Url = "https://mc-cdn.xyz/mosintools/Firefox.exe", FileName = "FireFoxSetup.exe" },
                new { CheckBox = operasel, Url = "https://mc-cdn.xyz/mosintools/OperaSetup.exe", FileName = "OperaSetup.exe" },
                new { CheckBox = bravesel, Url = "https://mc-cdn.xyz/mosintools/Brave.exe", FileName = "BraveSetup.exe" },

                new { CheckBox = battlenetsel, Url = "https://downloader.battle.net/download/getInstaller?os=win&installer=Battle.net-Setup.exe", FileName = "Battle.net-Setup.exe" },
                new { CheckBox = valorantsel, Url = "https://valorant.secure.dyn.riotcdn.net/channels/public/x/installer/current/live.live.ap.exe", FileName = "Install_VALORANT.exe" },
                new { CheckBox = leagueoflegendsel, Url = "https://lol.secure.dyn.riotcdn.net/channels/public/x/installer/current/live.kr.exe", FileName = "Install_League_of_Legends_kr.exe" },
                new { CheckBox = runeterrasel, Url = "https://bacon.secure.dyn.riotcdn.net/channels/public/x/installer/current/live.live.asia.exe", FileName = "Legends_Of_Runeterra_Installer.exe" },
                new { CheckBox = nexonplugsel, Url = "https://platform.nexon.com/launcher/installer/win/KRPC/Install_NexonPlug.exe", FileName = "Install_NexonPlug.exe" },
                new { CheckBox = epicgamesel, Url = "https://epicgames-download1.akamaized.net/Builds/UnrealEngineLauncher/Installers/Win32/EpicInstaller-17.2.0.msi?launcherfilename=EpicInstaller-17.2.0.msi", FileName = "EpicInstaller.msi" },
                new { CheckBox = steamsel, Url = "https://cdn.fastly.steamstatic.com/client/installer/SteamSetup.exe", FileName = "SteamSetup.exe" },
                new { CheckBox = uplaysel, Url = "https://static3.cdn.ubi.com/orbit/launcher_installer/UbisoftConnectInstaller.exe", FileName = "UbisoftConnectInstaller.exe" },
                new { CheckBox = eaappsel, Url = "https://origin-a.akamaihd.net/EA-Desktop-Client-Download/installer-releases/EAappInstaller.exe", FileName = "EAappInstaller.exe" },
                new { CheckBox = gogsel, Url = "https://webinstallers.gog-statics.com/download/GOG_Galaxy_2.0.exe?payload=H7DuiC0a--N4ivWpeWTnB57n0dgYWSEywh0MRIyAimHhth7t7BE_j822Lbvf-pRBB1GJA5REHcmAJ0YCWF1jKo0q00UH_nFE1XVLOvYBNoQvdBR_mh5dFboEO4d9jWB-NQtDuC1YmjOzHIlpAaRhmuThnauoxODHdjs_bXlVpXCkbBNz544Kz682xZ-NeinKMeKMIbhPDPANJoQqDXC0qaboiu6KdPbKyuMK1M1OdYWG5cEtaFzdtSxpZoJbcmrTzkDLhGCnU00IREJf_ap4Sufr6JNTV20HnNKYJvKrhA0auM0Pl16pLuPrZPszKZOaPTIHMFSzAOY9_Y_0qwdAMhPXLh5ZN4-fTmnnQeKXUPhzkyFT12EPYbK0axKw2aobOUzrC4k3N39zF3lX6jgBtVDbbHerVd8iPSCNFbB9BzzUnp5DWHnAQi4nvJ1otmwI35Ef1B5cXVzGIAAjcCVVeeo8dLbppD-H094aQrRxKE4m8JIA-p6E3tuaw0ldqD-L-DwTXPfoQmsbL-oKihmzdgMXbD2tB6hrR6vRDIidX5wNQ6D6-4Ho5gJSVBNXQOGGBYHgckgbu9745ygTrZDk81QDW5-2QNwdR9q5JtqSk9ViDrXiwHr2hcqQhuglR3JlFFJUsqj7sAVdA9lBqB6mkVigclwq00SZSqBnPHMRjb3p3qCzCftw_LIp-WzCOU5XhLU1txYLdFs25xHzKQYfhBDVpXEHf2Yp1Z_4LkNwuyn2sNIxpe3xG78B7Jb8tm-4S_yHugCP", FileName = "GOG_Galaxy_2.0.exe" },
                new { CheckBox = xboxsel, Url = "https://dlassets-ssl.xboxlive.com/public/content/XboxInstaller/XboxInstaller.exe", FileName = "XboxInstaller.exe" },
                new { CheckBox = rockstarsel, Url = "https://gamedownloads.rockstargames.com/public/installer/Rockstar-Games-Launcher.exe?_gl=1*1rqxcs9*_gcl_au*OTI4NzA4MzcyLjE3Mzc1NDg5OTY.*_ga*NDQxOTcwMzY5LjE3Mzc1NDg5ODc.*_ga_PJQ2JYZDQC*MTczNzU0ODk4Ny4xLjEuMTczNzU0ODk5Ni4wLjAuMA..", FileName = "Rockstar-Games-Launcher.exe" },
                new { CheckBox = itchsel, Url = "https://itch.io/app/download?platform=windows", FileName = "itch-setup.exe" },
                new { CheckBox = minecraftsel, Url = "https://launcher.mojang.com/download/MinecraftInstaller.exe?ref=mcnet", FileName = "MinecraftInstaller.exe" },

                new { CheckBox = hancomsel, Url = "https://mc-cdn.xyz/mosintools/%ed%95%9c%ec%bb%b4%ec%98%a4%ed%94%bc%ec%8a%a42024.zip", FileName = "한컴오피스 2024.zip" },
                new { CheckBox = libreofficesel, Url = "https://ftp.kaist.ac.kr/tdf/libreoffice/stable/24.8.4/win/x86_64/LibreOffice_24.8.4_Win_x86-64_sdk.msi", FileName = "libreoffice_24.8.4.msi" },
                new { CheckBox = polarissel, Url = "https://install.polarisoffice.com/pcoffice/PolarisOfficeSetup_service.exe", FileName = "PolarisOfficeSetup.exe" },
                new { CheckBox = wpsofficesel, Url = "https://wdl1.pcfg.cache.wpscdn.com/wpsdl/wpsoffice/download/12.2.0.19805/500.1001/WPSOffice_12.2.0.19805.exe", FileName = "WPSOffice_12.2.0.19805.exe" },
                new { CheckBox = freeofficesel, Url = "https://mc-cdn.xyz/mosintools/freeoffice2024.msi", FileName = "Freeoffice2024.msi" },
                new { CheckBox = activationsel, Url = "https://mc-cdn.xyz/mosintools/Activations.cmd", FileName = "윈도우정품인증.cmd" },
                new { CheckBox = discordsel, Url = "https://discord.com/api/downloads/distributions/app/installers/latest?channel=stable&platform=win&arch=x64", FileName = "DiscordSetup.exe" },
                new { CheckBox = kakaosel, Url = "https://app-pc.kakaocdn.net/talk/win32/KakaoTalk_Setup.exe", FileName = "KakaoTalk_Setup.exe" },
                new { CheckBox = telegramsel, Url = "https://telegram.org/dl/desktop/win64", FileName = "TelegramSetup.exe" },
                new { CheckBox = jdk23sel, Url = "https://download.oracle.com/java/23/latest/jdk-23_windows-x64_bin.msi", FileName = "jdk-23_windows-x64_bin.msi" },
                new { CheckBox = runtime9sel, Url = "https://download.visualstudio.microsoft.com/download/pr/24046c49-1b56-4c1b-9c15-75c94d7a841a/d089fe00210b8113c33ea96e1e932fb7/dotnet-runtime-9.0.1-win-x64.exe", FileName = "dotnet-runtime-9.0.1-win-x64.exe" },
                new { CheckBox = sdk9sel, Url = "https://download.visualstudio.microsoft.com/download/pr/5f46239c-783c-4d49-a4a2-cd5b0a47ec51/9b72af54efd90a3874b63e4dd43855e7/dotnet-sdk-9.0.102-win-x64.exe", FileName = "dotnet-sdk-9.0.102-win-x64.exe" },
                new { CheckBox = dxwebsel, Url = "https://download.microsoft.com/download/1/7/1/1718CCC4-6315-4D8E-9543-8E28A4E18C4C/dxwebsetup.exe", FileName = "DirectXSetup.exe" }
            };

            // ProgressBar 초기화
            progressBar.Value = 0;
            progressBar.Maximum = fileLinks.Length;

            // 선택된 파일 다운로드
            foreach (var file in fileLinks)
            {
                if (file.CheckBox.Checked)
                {
                    lblStatus.Text = $"다운로드 중: {file.Url}";
                    Application.DoEvents(); // UI 업데이트
                    DownloadFileWithProgress(file.Url, Path.Combine(downloadFolder, file.FileName));
                    progressBar.Value += 1; // ProgressBar 업데이트
                }
            }

            lblStatus.Text = "모든 다운로드가 완료되었습니다.";
            MessageBox.Show("다운로드가 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void patchnote_Click(object sender, EventArgs e)
        {
            patchnote patchnoteform = new patchnote();
            patchnoteform.Show();
        }

        private void credit_Click(object sender, EventArgs e)
        {
            credit creditform = new credit();
            creditform.Show();
        }
    }
}
