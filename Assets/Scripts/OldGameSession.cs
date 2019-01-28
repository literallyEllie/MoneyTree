using System.IO;
using System.Collections.Generic;

public class OldGameSession {

    public static readonly string FILE_NAME = "last_session.json";

    public string LastLevel;
    public bool CompletedLastLevel;

    // OldGameSession contains information about the last game session that was played.
    public OldGameSession()
    {
    }

    // Constructor allowing for the reading of the file.
    public OldGameSession(TextReader reader)
    {
        if (reader == null)
            return;

        try
        {
            var fileContents = reader.ReadToEnd();

            OldGameSession oldGameSession = UnityEngine.JsonUtility.FromJson<OldGameSession>(fileContents);
            this.LastLevel = oldGameSession.LastLevel;
            this.CompletedLastLevel = oldGameSession.CompletedLastLevel;
        }
        finally
        {
            if (reader != null) reader.Close();
        }
    }

    // Returns if they completed the last level they played.
    public bool AutoProgress()
    {
        return CompletedLastLevel;
    }

}
