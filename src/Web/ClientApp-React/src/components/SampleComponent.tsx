// SampleComponent.tsx
import React from "react";

interface SampleComponentProps {
  title: string;
}

const SampleComponent: React.FC<SampleComponentProps> = ({ title }) => (
  <p>{title}</p>
);

export default SampleComponent;
