using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class Program
{
    private DiscordSocketClient _client = null!;

    public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();


    public async Task MainAsync()
    {
        //설정 파일 (appsettings.json 등) 및 User Secrets에서 설정을 읽어오도록 구성
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>() //User Secrets 사용
            .Build();

        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers
        };

        _client = new DiscordSocketClient(config);

        //이벤트 구독
        _client.Log += Log;

        //메시지 수신 이벤트를 HandleMessageAsync 메서드와 연결
        _client.MessageReceived += HandleMessageAsync;

        var token = configuration["DiscordBotToken"];

        //봇 로그인
        await _client.LoginAsync(TokenType.Bot, token);

        //봇 서비스 시작
        await _client.StartAsync();

        //봇 꺼지지 않도록 무한 대기
        await Task.Delay(-1);
    }

    //
    private async Task HandleMessageAsync(SocketMessage message)
    {
        //메시지를 보낸 유저가 봇이 아닐 때만 처리
        if (message.Author.IsBot)
        {
            return;
        }

        //메시지 내용이 "!안녕"일 경우
        if (message.Content == "!안녕")
        {
            //응답
            await message.Channel.SendMessageAsync("반가워요!");
        }
    }

    //디스코드 봇의 내부 활동 상태를 콘솔창에 출력
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}