namespace Application.Interfaces; 
public interface ICsvFileBuilder {
    byte[] BuildCsvFile<T>(IEnumerable<T> records);
} 
