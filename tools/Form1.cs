using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Security.Principal;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using static System.Net.WebRequestMethods;

namespace tools
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            if (!IsUserAdministrator())
            {
                // 권한 부족 메시지 표시
                MessageBox.Show(
                    "이 프로그램은 관리자 권한이 필요합니다.\n관리자 권한으로 다시 실행해 주세요.",
                    "권한 부족",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                // 애플리케이션 강제 종료
                Environment.Exit(0); // Application.Exit 대신 Environment.Exit 사용
                return;
            }

            InitializeComponent();
        }

        private bool IsUserAdministrator()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnDownload_Click_1(object sender, EventArgs e)
        {
            // 다운로드할 파일 URL 목록
            string[] fileUrls = {
                "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7B02A14BF6-92F1-8E0D-4199-1917FDCBA9D8%7D%26lang%3Dko%26browser%3D4%26usagestats%3D1%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26ap%3Dx64-statsdef_1%26installdataindex%3Dempty/update2/installers/ChromeSetup.exe",
                "https://dl.bandisoft.com/bandizip.std/BANDIZIP-SETUP-STD-X64.EXE?13",
                "https://dl.bandisoft.com/bandiview/BANDIVIEW-SETUP-X64.EXE?75",
                "https://app-pc.kakaocdn.net/talk/win32/KakaoTalk_Setup.exe",
                "https://stable.dl2.discordapp.net/distro/app/stable/win/x64/1.0.9175/DiscordSetup.exe",
                "https://valorant.secure.dyn.riotcdn.net/channels/public/x/installer/current/live.live.ap.exe",
                "https://cdn.fastly.steamstatic.com/client/installer/SteamSetup.exe",
                "https://platform.nexon.com/launcher/installer/win/KRPC/Install_NexonPlug.exe",
                "https://download.visualstudio.microsoft.com/download/pr/38e45a81-a6a4-4a37-a986-bc46be78db16/33e64c0966ebdf0088d1a2b6597f62e5/dotnet-sdk-9.0.101-win-x64.exe",
                "https://vscode.download.prss.microsoft.com/dbazure/download/stable/fabdb6a30b49f79a7aba0f2ad9df9b399473380f/VSCodeUserSetup-x64-1.96.2.exe",
                "https://download.oracle.com/java/23/latest/jdk-23_windows-x64_bin.msi",
                "https://download.scdn.co/SpotifySetup.exe",
                "https://mc-cdn.xyz/hancomoffice.zip",
                "https://mc-cdn.xyz/KMS_Activation.cmd"
            };

            // 바탕화면의 bin 폴더 경로
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string binFolderPath = Path.Combine(desktopPath, "bin");

            // bin 폴더가 없으면 생성
            if (!Directory.Exists(binFolderPath))
            {
                Directory.CreateDirectory(binFolderPath);
            }

            // ProgressBar 초기화
            progressBarDownload.Value = 0;
            progressBarDownload.Maximum = 100;  // 전체 진행률을 100%로 설정
            progressBarDownload.Visible = true;
            labelProgress.Text = "현재 상태: 0%";

            try
            {
                int downloadedFiles = 0;
                int totalFiles = fileUrls.Length;

                using (WebClient webClient = new WebClient())
                {
                    // DownloadProgressChanged 이벤트 핸들러 추가
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((s, eProgress) =>
                    {
                        // 각 파일의 진행률을 계산하여 0-100 범위의 진행률로 업데이트
                        int progressPercentage = (int)((eProgress.BytesReceived * 100) / eProgress.TotalBytesToReceive);

                        // 전체 진행률 계산 (파일이 완료되면 100%로 표시)
                        int overallProgress = (int)(((downloadedFiles + (progressPercentage / 100.0)) / totalFiles) * 100);
                        progressBarDownload.Value = overallProgress;
                        labelProgress.Text = $"현재 상태: {overallProgress}%";
                    });

                    // 다운로드할 각 파일에 대해 순차적으로 다운로드
                    foreach (string fileUrl in fileUrls)
                    {
                        string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
                        string filePath = Path.Combine(binFolderPath, fileName);

                        // 비동기 파일 다운로드
                        webClient.DownloadFileAsync(new Uri(fileUrl), filePath);

                        // 다운로드가 끝날 때까지 대기
                        while (webClient.IsBusy)
                        {
                            Application.DoEvents();
                        }

                        // 다운로드 완료 후, 다운로드된 파일 수 업데이트
                        downloadedFiles++;
                    }
                }

                MessageBox.Show("모든 파일 다운로드가 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 다운로드 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 다운로드 완료 후 ProgressBar 숨기기
                labelProgress.Text = "모든 파일 다운로드가 완료되었습니다.";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 링크 열기
            string url = "https://cwallet.com/t/IE3QJUS1";
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // 링크를 기본 브라우저로 열기 위해 필요
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open link: {ex.Message}");
            }
        }

        private void btnDriver_Click(object sender, EventArgs e)
        {
            // 링크 열기
            string url = "https://www.nvidia.com/ko-kr/drivers/";
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // 링크를 기본 브라우저로 열기 위해 필요
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open link: {ex.Message}");
            }
        }

        private void btnPower_Click(object sender, EventArgs e)
        {

        }

        private void progressBarDownload_Click(object sender, EventArgs e)
        {

        }
    }
}
