using BlockuMoveEvaluator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BlockuMoveEvaluator
{
    public static class SaveLoad
    {
        public static void SaveSuggestionDataToXML(this MoveEvaluationModel MoveEvaluation, bool bWriteSuggestionToFile, string filepath)
        {
            if (!bWriteSuggestionToFile)
                return;

            string directory = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            XmlSerializer serializer = new XmlSerializer(typeof(MoveEvaluationModel));
            TextWriter writer = new StreamWriter(filepath);
            serializer.Serialize(writer, MoveEvaluation);
            writer.Close();
        }

        public static MoveEvaluationModel LoadSuggestionFromXML(string filepath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MoveEvaluationModel));

            if (!File.Exists(filepath))
                return new MoveEvaluationModel();

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
                return (MoveEvaluationModel)serializer.Deserialize(fs);
        }
    }
}
