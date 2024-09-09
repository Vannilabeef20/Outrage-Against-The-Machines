using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;

namespace Game
{
    /// <summary>
    /// Static class that enchances Unity's Debug.Log<br/>
    /// allowing for filtering and better readability.<br/>
    /// [TEMPORARY] also holds IsDebugModeEnabled.
    /// </summary>
	public static class CustomLogger
    {
        public static bool IsDebugModeEnabled;

        //Debug filters
        public static EDebugSubjectFlags DebugSubjects;
        public static EDebugTypeFlags DebugTypes;

        #region Debug Logs

        /// <summary>
        /// Logs a custom formatted message to the console,<br/>
        /// Shows a message along with its Sender and "Subject filter".
        /// </summary>
        /// <param name="Sender">The object that sent the log request.</param>
        /// <param name="Message">The message to be displayed on the console.</param>
        /// <param name="EDebugSubject">Enum Flag "Subject filter".</param>
        public static void Log(this object Sender, object Message,
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Testing)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.Log) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.Log($"<color=lime>{Message}</color>\n" +
                    $" <color=magenta>{EDebugSubject}</color><color=aqua> Sender: {Sender}</color>");
            }
        }

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="Sender">The object that sent the log request.</param>
        /// <param name="Message">The information to be displayed on the log.</param>
        /// <param name="EDebugSubject">EFlag for filtering.</param>
        public static void LogWarning(this object Sender, string Message,
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Testing)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.LogWarning) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.LogWarning($"<color=yellow>{Message}</color>\n" +
                    $" <color=magenta>{EDebugSubject}</color><color=aqua> Sender: {Sender}</color>");
            }
        }

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="Sender">The object that sent the log request.</param>
        /// <param name="Message">The information to be displayed on the log.</param>
        /// <param name="EDebugSubject">EFlag for filtering.</param>
        public static void LogError(this object Sender, string Message,
            EDebugSubjectFlags EDebugSubject = EDebugSubjectFlags.Testing)
        {
            if (DebugTypes.HasAnyFlag(EDebugTypeFlags.LogError) && DebugSubjects.HasAnyFlag(EDebugSubject))
            {
                Debug.LogError($"<color=red>{Message}</color>\n" +
                    $" <color=magenta>{EDebugSubject}</color><color=aqua> Sender: {Sender}</color>");
            }
        }
        #endregion
    }
}