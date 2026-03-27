using System.Xml;
using System.Xml.Linq;
using NeoKolors.Settings.Builder.Info;

namespace NeoKolors.Settings.Builder.Xml;

public class XmlSettingsReader<TResult> {
    private readonly SettingsNode<TResult> _node;
    
    public XmlSettingsReader(SettingsNode<TResult> node) {
        _node = node;
    }

    /// <summary>
    /// Reads an XML file and sets the individual arguments to the stored node
    /// </summary>
    /// <param name="filePath">Path to the XML file to read</param>
    /// <exception cref="FileNotFoundException">Thrown when the XML file doesn't exist</exception>
    /// <exception cref="XmlException">Thrown when the XML is malformed</exception>
    public void ReadXmlFile(string filePath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException($"XML file not found: {filePath}");
        }

        try {
            var xmlDoc = XDocument.Load(filePath);
            var root = xmlDoc.Root;

            if (root is null) {
                throw new XmlException("XML file has no root element");
            }

            ReadRoot(root);
        }
        catch (XmlException) {
            throw;
        }
        catch (System.Exception ex) {
            throw new XmlException($"Error reading XML file: {ex.Message}", ex);
        }
    }

    private void ReadRoot(XElement root) {
        var elements = root.Nodes().ToArray();
            
        if (_node.Elements.Length != elements.Count())
            throw new XmlException("XML file does not match the number of elements in the target settings node.");
            
        for (int i = 0; i < elements.Length; i++) {
            switch (_node.Elements[i]) {
                case ArgumentInfo ai:
                    SetArg(ai, elements[i]);
                    break;
                case SettingsMethodGroupInfo gi:
                    SetGroup(gi, elements[i]);
                    break;
            }
        }
    }

    private void SetArg(ArgumentInfo i, XNode n) {
        if (n is not XElement a)
            throw new XmlException($"Argument '{i.Name}' does not have a corresponding element.");
                    
        i.Value.Set(a.Value);
    }

    private void SetGroup(SettingsMethodGroupInfo i, XNode n) {
        if (n is not XElement g)
            throw new XmlException($"Group '{i.Name}' does not have a corresponding element.");

        
    }
}