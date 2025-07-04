using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager Instance;
    private bool eventsInitialized = false;

     private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

    }

    public async void StartClientService()
    {
        try
        {
            if(UnityServices.State != ServicesInitializationState.Initialized)
            {
                var options = new InitializationOptions();
                options.SetProfile("default_profile");
                await UnityServices.InitializeAsync();
            }

            if(!eventsInitialized)
            {
                SetupEvents();
            }

            if(AuthenticationService.Instance.SessionTokenExists)
            {
                SignInAnonymouslyAsync();
            }
        }
        catch (Exception exception)
        {

        }
    }

    public async void SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException exception)
        {

        }
        catch(RequestFailedException exception)
        {

        }
    }

    private void SetupEvents()
    {
        eventsInitialized = true;

        AuthenticationService.Instance.SignedIn += () => 
        {

        };

        AuthenticationService.Instance.SignedOut += () => 
        {

        };

        AuthenticationService.Instance.Expired += () => 
        {

        };
        
    }
}
