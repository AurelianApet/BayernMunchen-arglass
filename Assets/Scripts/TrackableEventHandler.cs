/*============================================================================== 
 * Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using Vuforia;
using System;

using System.Collections.Generic;
using System.Collections;
/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class TrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PRIVATE_MEMBERS
    private TrackableBehaviour mTrackableBehaviour;
    private bool mHasBeenFound = false;
    private bool mLostTracking;
    private float mSecondsSinceLost;
    #endregion // PRIVATE_MEMBERS

	//khs add
	public GameObject mLogo;
	public GameObject mEffects;
	public GameObject mAudio;
	public GameObject mVideo;

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        OnTrackingLost();
    }

    void Update()
    {
        // Pause the video if tracking is lost for more than two seconds
        if (mHasBeenFound && mLostTracking)
        {
            if (mSecondsSinceLost > 1.0f)
            {
                VideoPlaybackBehaviour video = GetComponentInChildren<VideoPlaybackBehaviour>();
                if (video != null &&
                    video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
                {
                    video.VideoPlayer.Pause();
				}
                mLostTracking = false;
            }

            mSecondsSinceLost += Time.deltaTime;
        }

		//khs add
    }

    #endregion //MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
			//play audio
			if(mLogo.activeSelf == true)
				mAudio.GetComponent<AudioSource>().Play();
        }
        else
        {
            OnTrackingLost();
			//stop audio
			mAudio.GetComponent<AudioSource>().Stop();
        }
    }
    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS
    private void OnTrackingFound()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
        Collider[] colliderComponents = GetComponentsInChildren<Collider>();

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = true;
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");

        // Optionally play the video automatically when the target is found

        VideoPlaybackBehaviour video = GetComponentInChildren<VideoPlaybackBehaviour>();
        if (video != null && video.AutoPlay)
        {
            if (video.VideoPlayer.IsPlayableOnTexture())
            {
                VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
				if (state == VideoPlayerHelper.MediaState.PAUSED || 
					state == VideoPlayerHelper.MediaState.READY ||
					state == VideoPlayerHelper.MediaState.STOPPED)
                {
                    // Pause other videos before playing this one
                    PauseOtherVideos(video);
                    // Play this video on texture where it left off
                    video.VideoPlayer.Play(false, video.VideoPlayer.GetCurrentPosition());
                }
                else if (state == VideoPlayerHelper.MediaState.REACHED_END)
                {
                    // Pause other videos before playing this one
                    PauseOtherVideos(video);
                    // Play this video from the beginning
                    video.VideoPlayer.Play(false, 0);
					mLogo.SetActive (true);
					mEffects.SetActive (true);
					mAudio.GetComponent<AudioSource>().Play();
					mVideo.SetActive (false);
                }
            }
        }

        mHasBeenFound = true;
        mLostTracking = false;
    }

    private void OnTrackingLost()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
        Collider[] colliderComponents = GetComponentsInChildren<Collider>();

        // Disable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;
        }

        // Disable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = false;
        }

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");

        mLostTracking = true;
        mSecondsSinceLost = 0;
    }

    // Pause all videos except this one
    private void PauseOtherVideos(VideoPlaybackBehaviour currentVideo)
    {
        VideoPlaybackBehaviour[] videos = (VideoPlaybackBehaviour[])
                FindObjectsOfType(typeof(VideoPlaybackBehaviour));

        foreach (VideoPlaybackBehaviour video in videos)
        {
            if (video != currentVideo)
            {
                if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
                {
                    video.VideoPlayer.Pause();
                }
            }
        }
    }
    #endregion //PRIVATE_METHODS
}
