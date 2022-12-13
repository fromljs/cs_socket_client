using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace cs_socket_client
{
    internal class Program
    {
        // 실행 함수
        private static void Main(string[] args)
        {
            // Socket EndPoint 설정
            var ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            // 소켓 인스턴스 생성
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // 소켓 접속
                client.Connect(ipep);
                // 접속이 되면 Task로 병렬 처리
                new Task(() =>
                {
                    try
                    {
                        // 종료되면 자동 client 종료
                        // 무한 루프
                        while (true)
                        {
                            // 통신 바이너리 버퍼
                            var binary = new Byte[1024];
                            // 서버로부터 메시지 대기
                            client.Receive(binary);
                            // 서버로 받은 메시지를 String으로 변환
                            var data = Encoding.ASCII.GetString(binary).Trim('\0');
                            // 메시지 내용이 공백이라면 계속 메시지 대기 상태로
                            if (String.IsNullOrWhiteSpace(data))
                            {
                                continue;
                            }
                            // 메시지 내용을 콘솔에 표시
                            Console.Write(data);
                        }
                    }
                    catch (SocketException)
                    {
                        // 접속 끝김이 발생하면 Exception이 발생
                    }
                    // Task 실행
                }).Start();
                // 유저로부터 메시지 받기 위한 무한 루프
                while (true)
                {
                    // 콘솔 입력 받는다.
                    var msg = Console.ReadLine();
                    // 클라이언트로 받은 메시지를 String으로 변환
                    client.Send(Encoding.ASCII.GetBytes(msg + "\r\n"));
                    // 메시지 내용이 exit라면 무한 루프 종료(즉, 클라이언트 종료)
                    if ("EXIT".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
                // 콘솔 출력 - 접속 종료 메시지
                Console.WriteLine($"Disconnected");
            }
            // 아무 키나 누르면 종료
            Console.WriteLine("Press Any key...");
            Console.ReadLine();
        }
    }
}