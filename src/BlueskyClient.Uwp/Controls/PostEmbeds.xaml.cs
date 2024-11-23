﻿using Bluesky.NET.Models;
using BlueskyClient.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

#nullable enable

namespace BlueskyClient.Controls;

public sealed partial class PostEmbeds : UserControl
{
    private readonly IImageViewerService _imageViewerService;

    public static readonly DependencyProperty EmbedProperty = DependencyProperty.Register(
        nameof(Embed),
        typeof(PostEmbed),
        typeof(PostEmbeds),
        new PropertyMetadata(null, OnEmbedChanged));

    private static void OnEmbedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PostEmbeds p)
        {
            p.Bindings.Update();
            p.TryLoadVideo();
        }
    }

    public PostEmbeds()
    {
        this.InitializeComponent();
        _imageViewerService = App.Services.GetRequiredService<IImageViewerService>();
    }

    public PostEmbed? Embed
    {
        get => (PostEmbed?)GetValue(EmbedProperty);
        set => SetValue(EmbedProperty, value);
    }

    private bool IsVideo => Embed?.Playlist is { Length: > 0 };

    private bool IsExternalUrl => Embed?.External?.Uri is not null;

    private string ExternalThumb => Embed?.External?.Thumb ?? "http://localhost";

    private bool IsSingleImageEmbed => Embed?.Images?.Length == 1;

    private bool IsMultiImageEmbed => Embed?.Images?.Length > 1;

    public IReadOnlyList<ImageEmbed> MultiImages => Embed?.Images ?? [];

    private ImageEmbed? SingleImage => Embed?.Images is [ImageEmbed image, ..]
        ? image
        : null;

    private double SingleImageMaxWidth =>
        SingleImage?.AspectRatio is { Height: double height, Width: double width } && height > width
        ? 300
        : double.PositiveInfinity;

    private void TryLoadVideo()
    {
        if (Embed?.Playlist is { Length: > 0 } sourceUrl && 
            Uri.TryCreate(sourceUrl, UriKind.Absolute, out Uri videoSource))
        {
            VideoPlayer.Source = MediaSource.CreateFromUri(videoSource);

            if (Embed?.Thumbnail is { Length: > 0 } thumbUrl && 
                Uri.TryCreate(thumbUrl, UriKind.Absolute, out Uri thumbSource))
            {
                VideoPlayer.PosterSource = new BitmapImage(thumbSource);
            }
        }
    }

    private async void OnExternalUrlClicked(object sender, RoutedEventArgs e)
    {
        if (Embed?.External?.Uri is string { Length: > 0 } uri &&
            Uri.TryCreate(uri, UriKind.Absolute, out Uri result))
        {
            await Launcher.LaunchUriAsync(result);
        }
    }

    private void OnGridViewImageClicked(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not ImageEmbed imageClicked)
        {
            return;
        }

        var index = 0;

        foreach (var image in MultiImages)
        {
            if (image == imageClicked)
            {
                break;
            }

            index++;
        }

        if (index < MultiImages.Count)
        {
            _imageViewerService.RequestImageViewer(MultiImages, launchIndex: index);
        }
    }

    private void OnSingleImageClicked(object sender, TappedRoutedEventArgs e)
    {
        if (SingleImage is { } image)
        {
            _imageViewerService.RequestImageViewer([image]);
        }
    }

    private void OnVideoSurfaceTapped(object sender, TappedRoutedEventArgs e)
    {
        if (e.OriginalSource is Grid &&
            sender is MediaPlayerElement { MediaPlayer: MediaPlayer { PlaybackSession.PlaybackState: MediaPlaybackState state } mp })
        {
            e.Handled = true;
            if (state is MediaPlaybackState.Playing)
            {
                mp.Pause();
            }
            else if (state is MediaPlaybackState.Paused)
            {
                mp.Play();
            }
        }
    }

    private void OnVideoSurfaceKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (sender is MediaPlayerElement { IsFullWindow: true } mpe && e.Key is VirtualKey.Escape)
        {
            mpe.IsFullWindow = false;
            e.Handled = true;
        }
    }
}
