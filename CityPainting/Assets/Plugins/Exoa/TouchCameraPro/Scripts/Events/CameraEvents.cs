using UnityEngine;

namespace Exoa.Events
{
	public class CameraEvents
	{
		public enum Action { SwitchPerspective, DisableCameraMoves, Help, ResetCameraPositionRotation };

		public delegate void OnCameraBoolEvent(Action action, bool active);
		public delegate void OnCameraGameObjectEvent(GameObject obj);
		public delegate void OnCameraGameObjectBoolEvent(GameObject obj, bool focusOnFollow);
		public delegate void OnSwitchPerspectiveHandler(bool orthoMode);

		public static OnSwitchPerspectiveHandler OnBeforeSwitchPerspective;
		public static OnSwitchPerspectiveHandler OnAfterSwitchPerspective;
		public static OnCameraBoolEvent OnRequestButtonAction;
		public static OnCameraGameObjectEvent OnRequestObjectFocus;
		public static OnCameraGameObjectBoolEvent OnRequestObjectFollow;


	}
}
