namespace Repro

open System

open XamarinForms.Reactive.FSharp

open Xamarin.Forms.Platform.Android
open Xamarin.Forms

open Android.Graphics.Drawables
open Android.Content.PM
open Android.Runtime
open Android.Content
open Android.App
open Android

open ReactiveUI

open Splat

open System.IO
open XamarinForms.Reactive.FSharp

type Resources = Repro.Resource

type AppLaunchViewModel(?host: IScreen) = 
    inherit ReactiveObject()
    let host = LocatorDefaults.LocateIfNone host
    interface IRoutableViewModel with
        member __.HostScreen = host
        member __.UrlPathSegment = "Home"

type ReproPlatform(context: Activity) =
    interface IPlatform with
        member __.GetLocalFilePath fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName)
        member __.HandleAppLinkRequest _ = ()
        member __.RegisterDependencies _ = ()

module AppBootstrapping =
    let LandingPage() =
        let platform = Locator.Current.GetService<IPlatform>()
        new AppLaunchViewModel() :> IRoutableViewModel
    let CreateApp(platform, context) = new App<IPlatform>(platform, context, LandingPage) 

[<Activity(Label = "Issue Repro", ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true)>]
type MainActivity () =
    inherit FormsApplicationActivity ()
    let mutable reproPlatform = Unchecked.defaultof<ReproPlatform>
    override this.OnCreate (bundle) =
        base.OnCreate (bundle)
        AppDomain.CurrentDomain.UnhandledException.Subscribe(fun ex ->
            ()
        ) |> ignore
        Forms.Init(this, bundle)
        reproPlatform <- new ReproPlatform(this)
        let application = AppBootstrapping.CreateApp(reproPlatform, new UiContext(this))
        this.LoadApplication(application)
