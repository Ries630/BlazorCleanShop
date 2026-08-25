using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BlazorCleanShop.Api;

/// <summary>
/// Minimal API エンドポイントの登録処理を提供します。
/// </summary>
public static class ApiEndpointRouteBuilderExtensions
{
    /// <summary>
    /// アプリケーションが公開する Minimal API エンドポイントを登録します。
    /// </summary>
    /// <param name="endpoints">エンドポイントを登録するルートビルダー。</param>
    /// <returns>登録先のルートビルダー。</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="endpoints"/> が <see langword="null"/> の場合にスローされます。
    /// </exception>
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/api/status", () => TypedResults.Ok("ok"))
            .WithName("GetApiStatus")
            .WithSummary("API の稼働状態を取得します。")
            .WithDescription("Blazor UI と同じ ASP.NET Core ホストから応答します。");

        return endpoints;
    }
}
