using data.RemoteData.RemoteDataBase;
using data.Repository;
using domain.UseCase;
using Microsoft.Extensions.DependencyInjection;
using UI;


IServiceCollection services = new ServiceCollection();

services
    .AddDbContext<RemoteDatabaseContext>()
    .AddSingleton<IGroupRepository, SQLGroupRepositoryImpl>()
    .AddSingleton<IUserRepository, SQLUserRepositoryImpl>()
    .AddSingleton<IPresenceRepository, SQLPresenceRepositoryImpl>()
    .AddSingleton<UserUseCase>()
    .AddSingleton<GroupUseCase>()
    .AddSingleton<PresenceUseCase>()
    .AddSingleton<MainConsole>();



var serviceProvider = services.BuildServiceProvider();
MainConsole mainMenuUI = serviceProvider.GetService<MainConsole>();

mainMenuUI.Run();